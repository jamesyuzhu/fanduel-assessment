using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Services.Interface;
using System.Threading.Tasks;
using System;
using System.Linq;
using DepthChart.Api.Exceptions;

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
        public async Task<PlayerResponse> RemovePlayerFromDepthChartAsync(RemovePlayerFromDepthChartRequest request, string teamCode, DateTime? chartDate = null)
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

            // PlayerId is required
            if (request.PlayerId == null)
            {
                throw new ArgumentNullException(nameof(request.PlayerId));
            }

            // Calculate WeekStartDate
            var weekStartDate = chartDate ?? GetWeekStartDate();

            // Find all the positionDepth records of the current chart
            var positionDepthList = await GetAllPositionDepthByPositionAsync(teamCode, weekStartDate, request.PositionCode);

            // Find the target ChartPositionDepth record
            var target = positionDepthList.FirstOrDefault(x =>
                x.PlayerId == request.PlayerId.Value);

            // Throw exception if that player is not listed in the depth chart at that position
            if (target == null)
            {
                throw new PlayerNotInPositionException($"Position: {request.PositionCode}; PlayerId: {request.PlayerId}");
            }

            // Find the subsequent record, decrement their depth
            var updateList = positionDepthList
                .Where(p => p.Depth > target.Depth)
                .ToList();

            if (updateList != null && updateList.Count > 0)
            {
                foreach (var updateItem in updateList)
                {
                    updateItem.Depth--;
                }
            }

            // Save to the repository
            await _repository.RemovePlayerFromDepthChartAsync(target, updateList);

            return new PlayerResponse
            {
                PlayerId = target.PlayerId,
                PlayerName = target.PlayerName
            };
        }
    }
}
