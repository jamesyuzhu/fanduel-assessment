using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Services.Interface;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace DepthChart.Api.Services
{
    public partial class NFLDepthChartService : IDepthChartService
    {
        /// <summary>
        /// Removes a player from the depth chart for a given position and returns that player 
        /// An empty list should be returned if that player is not listed in the depth chart at that position
        /// </summary>
        /// <param name="request">The given request</param>
        /// <param name="teamCode">The code of the target team</param>
        /// <returns>The removed player record</returns>        
        public async Task<RemovePlayerFromDepthChartResponse> RemovePlayerFromDepthChartAsync(RemovePlayerFromDepthChartRequest request, string teamCode)
        {
            if (string.IsNullOrEmpty(teamCode))
            {
                throw new ArgumentNullException("TeamCode is required");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // PositionCode is required
            if (request.PositionCode == null)
            {
                throw new ArgumentNullException(nameof(request.PositionCode));
            }

            // Calculate WeekStartDate
            DateTime weekStartDate = GetWeekStartDate();

            // Find all the positionDepth records of the current chart
            var positionDepthList = await GetAllPositionDepthByChartAsync(teamCode, weekStartDate);

            // Find the target ChartPositionDepth record
            var positionDepth = positionDepthList.FirstOrDefault(
                x => x.PositionCode.ToLowerInvariant() == request.PositionCode.ToLowerInvariant()
                && x.PlayerId == request.PlayerId);

            // Return empty response if that player is not listed in the depth chart at that position
            if (positionDepth == null) { return new RemovePlayerFromDepthChartResponse(); }

            // Remove the positionDepth record
            _context.ChartPositionDepths.Remove(positionDepth);

            // Find the subsequent record, decrement their depth
            var updateList = positionDepthList.Where(p => p.Depth > positionDepth.Depth).ToList();
            if (updateList.Count > 0)
            {
                foreach (var updateItem in updateList)
                {
                    updateItem.Depth--;
                }
            }
            await _context.SaveChangesAsync();

            return new RemovePlayerFromDepthChartResponse
            {
                PlayId = positionDepth.PlayerId,
                PlayName = positionDepth.PlayerName
            };
        }
    }
}
