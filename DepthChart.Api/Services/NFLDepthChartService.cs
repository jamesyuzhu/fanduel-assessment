using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Models = DepthChart.Api.Models;
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
    public class NFLDepthChartService : IDepthChartService
    {         
        private readonly DepthChartDbContext _context;

        public string SportCode => "NFL";
        
        /// <summary>
        /// Add those teams with their codes who have same logic for generating the DepthChart in NFL
        /// </summary>
        public List<string> TeamCodes => new List<string> { "TampaBayBuccaneers" };

        public NFLDepthChartService(DepthChartDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds a player to the depth chart at a given position 
        /// Adding a player without a position_depth would add them to the end of the depth chart at that position
        /// The added player would get priority.Anyone below that player in the depth chart would get moved down a
        /// position_depth
        /// </summary>
        /// <param name="request">The given request</param>
        /// <returns>The added ChartPositionDepth</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<AddPlayerToDepthChartResponse> AddPlayerToDepthChart(AddPlayerToDepthChartRequest request, string teamCode)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
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
            DateTime weekStartDate = GetWeekStartDate();

            // Find or create DepthChart
            Models.DepthChart depthChart = await GetDepthChart(request, weekStartDate, teamCode);

            // Find index and insert into the right position
            var positionDepthList = await _context.ChartPositionDepths
                .Where(c => c.DepthChartId == depthChart.Id)
                .ToListAsync();

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
                    positionDepthItem.Depth = positionDepthItem.Depth + 1;
                }
            }

            // Save ChartPositionDepth
            var chartPositionDepth = new Models.ChartPositionDepth
            {
                DepthChartId = depthChart.Id,
                PositionCode = request.PositionCode,
                PlayerId = request.PlayerId,
                PlayerName = request.PlayerName,
                Depth = depth
            };

            _context.ChartPositionDepths.Add(chartPositionDepth);
            if (updateList.Count > 0)
            {
                _context.ChartPositionDepths.UpdateRange(updateList);
            }
            await _context.SaveChangesAsync();

            return new AddPlayerToDepthChartResponse
            {
                DepthChartId = chartPositionDepth.DepthChartId,
                PositionCode = chartPositionDepth.PositionCode,
                PlayId = chartPositionDepth.PlayerId,
                PlayName = chartPositionDepth.PlayerName,
                Depth = chartPositionDepth.Depth,
                WeekStartDate = depthChart.WeekStartDate,
                TeamCode = depthChart.TeamCode
            };
        }

        private async Task<Models.DepthChart> GetDepthChart(AddPlayerToDepthChartRequest request, DateTime weekStartDate, string teamCode)
        {
            var depthChart = await _context.DepthCharts
                            .FirstOrDefaultAsync(d => d.TeamCode == teamCode && d.WeekStartDate == weekStartDate);

            if (depthChart == null)
            {
                depthChart = new Models.DepthChart
                {
                    Id = Guid.NewGuid(),
                    WeekStartDate = weekStartDate,
                    TeamCode = teamCode
                };
                _context.DepthCharts.Add(depthChart);
                await _context.SaveChangesAsync();
            }

            return depthChart;
        }

        private static DateTime GetWeekStartDate()
        {
            var currentDate = DateTime.Today;
            var weekStartDate = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            return weekStartDate;
        }
    }
}
