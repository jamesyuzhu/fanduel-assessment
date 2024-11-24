using System.Threading.Tasks;
using System;
using DepthChart.Api.Repositories;
using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using DepthChart.Api.Models;

namespace DepthChart.Api.Services
{
    /// <summary>
    /// The service is created for NFL sport specifically and implement the IDepthChartService
    /// If we have team of NFL requires specific custom logic for some operations, we can simply 
    /// create another concrete class inherited from IDepthChartService (like NFLTeamABCDepthChartService)
    /// </summary>
    public partial class NFLDepthChartService : IDepthChartService
    {         
        private readonly IDepthChartRepository _repository;

        public string SportCode => "NFL";
        
        /// <summary>
        /// Add those teams with their codes who have same logic for generating the DepthChart in NFL
        /// </summary>
        public List<string> TeamCodes => new List<string> { "TampaBayBuccaneers" };

        public NFLDepthChartService(IDepthChartRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Adds a player to the depth chart at a given position 
        /// Adding a player without a position_depth would add them to the end of the depth chart at that position
        /// The added player would get priority.Anyone below that player in the depth chart would get moved down a
        /// position_depth
        /// </summary>
        /// <param name="request">The given request</param>
        /// <param name="chartDate">The date that a chart is associated, e.g., a weekly chart may use the first week day.
        /// It is optional. If it is not present, the system will default to the current
        /// period, e.g., the first day of the current week</param>
        /// <returns>The added ChartPositionDepth</returns>      
        public async Task<AddPlayerToDepthChartResponse> AddPlayerToDepthChartAsync(AddPlayerToDepthChartRequest request, string teamCode, DateTime? chartDate = null)
        {
            if (string.IsNullOrEmpty(teamCode))
            {
                throw new ArgumentNullException("TeamCode is required");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            
            // Position is required
            if (string.IsNullOrEmpty(request.PositionCode))
            {
                throw new ArgumentNullException(nameof(request.PositionCode));
            }

            // PlayerName is required field
            if (string.IsNullOrEmpty(request.PlayerName))
            {
                throw new ArgumentNullException(nameof(request.PlayerName));
            }

            // PlayerId has to be greater than 0
            if (request.PlayerId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(request.PlayerId));
            }            

            // Depth has to be greater than 0
            if (request.Depth != null && request.Depth.Value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Depth));
            }

            // Calculate WeekStartDate
            DateTime weekStartDate = chartDate ?? GetWeekStartDate();          

            // Get a list of player for the given position in the current chart            
            var positionDepthList = await GetAllPositionDepthByPositionAsync(teamCode, weekStartDate, request.PositionCode);

            // If the depth is not presented, push it to the end of the chart position
            var depth = 0;
            var updateList = new List<ChartPositionDepth>();

            if (request.Depth == null || request.Depth.Value > positionDepthList.Count)
            {
                depth = positionDepthList.Count + 1;
            }
            else
            {
                depth = request.Depth.Value;
                // Find the element with depth greater or equal to the request.Depth
                // Increment the depth and update them
                updateList = positionDepthList.Where(p => p.Depth >= depth).ToList();
                foreach (var positionDepthItem in updateList)
                {
                    positionDepthItem.PlayerName = positionDepthItem.PlayerName;
                    positionDepthItem.Depth++;
                }
            }

            // Create new ChartPositionDepth
            var chartPositionDepth = new Models.ChartPositionDepth
            {
                SportCode = SportCode,
                TeamCode = teamCode,
                ChartDate = weekStartDate,
                PositionCode = request.PositionCode,
                PlayerId = request.PlayerId,
                PlayerName = request.PlayerName,
                Depth = depth
            };

            // Save to the repository
            await _repository.AddPlayerToDepthChartAsync(chartPositionDepth, updateList);

            return new AddPlayerToDepthChartResponse
            {
                SportCode = chartPositionDepth.SportCode,
                TeamCode = chartPositionDepth.TeamCode,
                PositionCode = chartPositionDepth.PositionCode,
                PlayerId = chartPositionDepth.PlayerId,
                PlayerName = chartPositionDepth.PlayerName,
                Depth = chartPositionDepth.Depth,
                WeekStartDate = chartPositionDepth.ChartDate,                
            };
        }        

        public static DateTime GetWeekStartDate()
        {
            var currentDate = DateTime.Today;
            var weekStartDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            return weekStartDate;
        }
       
        private async Task<List<ChartPositionDepth>> GetAllPositionDepthByPositionAsync(string teamCode, DateTime weekStartDate, string positionCode)
        {
            // Get a list of player for the given position in the current chart
            var fullList = await _repository.GetFullDepthChartAsync(SportCode, teamCode, weekStartDate);
            return fullList.Where(x => x.PositionCode.ToLowerInvariant() == positionCode.ToLowerInvariant()).ToList();
        }
    }
}
