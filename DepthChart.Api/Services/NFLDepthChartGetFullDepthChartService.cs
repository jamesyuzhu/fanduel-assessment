using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Services.Interface;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace DepthChart.Api.Services
{
    public partial class NFLDepthChartService : IDepthChartService
    {
        /// <summary>
        /// Print out the full depth chart with every position on the team and every player within the Depth Chart
        /// </summary>
        /// <param name="teamCode">The code of the target team</param>
        /// <param name="targetDate">The specified target date that one DepthChart is associated to. 
        /// It is optional parameter. If it is not present, we will use the current week start date as the target date</param>
        /// <returns>Every players with its depth in each position</returns>        
        public async Task<List<PositionDepthResponse>> GetFullDepthChart(string teamCode, DateTime? targetDate = null)
        {
            if (string.IsNullOrEmpty(teamCode))
            {
                throw new ArgumentNullException("TeamCode is required");
            }             

            // Calculate WeekStartDate
            DateTime weekStartDate = targetDate ?? GetWeekStartDate();

            // Find all the positionDepth records of the current chart
            var positionDepthList = await GetAllPositionDepthByChartAsync(teamCode, weekStartDate);
             
            return positionDepthList.Select(x =>
            {
                return new PositionDepthResponse
                {
                    PositionCode = x.PositionCode,
                    PlayerId = x.PlayerId,
                    PlayerName = x.PlayerName,
                    Depth = x.Depth
                };
            }).ToList();
        }
    }
}
