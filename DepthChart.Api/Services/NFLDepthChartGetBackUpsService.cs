using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Services.Interface;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
using DepthChart.Api.Exceptions;

namespace DepthChart.Api.Services
{
    public partial class NFLDepthChartService : IDepthChartService
    {
        /// <summary>
        /// For a given player and position, we want to see all players that are “Backups”, those with a lower position_depth 
        /// An empty list should be returned if the given player has no Backups 
        /// An empty list should be returned if the given player is not listed in the depth chart at that position
        /// <param name="request">The given request</param>
        /// <param name="teamCode">The code of the target team</param>
        /// <param name="chartDate">The specified target date that one DepthChart is associated to. It is optional parameter</param>
        /// <returns>The successor player records</returns>        
        public async Task<List<PlayerResponse>> GetBackupsAsync(GetBackUpsRequest request, string teamCode, DateTime? chartDate = null)
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
            DateTime weekStartDate = chartDate ?? GetWeekStartDate();

            // Find all the positionDepth records of the current chart
            var positionDepthList = await GetAllPositionDepthByPositionAsync(teamCode, weekStartDate, request.PositionCode);

            // Find the target ChartPositionDepth record
            var positionDepth = positionDepthList.FirstOrDefault(x =>
                x.PlayerId == request.PlayerId.Value);

            // Throw exception if that player is not listed in the depth chart at that position
            if (positionDepth == null) 
            {
                throw new PlayerNotInPositionException($"Position: {request.PositionCode}; PlayerId: {request.PlayerId}");
            }
             
            // Find the subsequent record, decrement their depth
            var targetList = positionDepthList
                .Where(p => p.Depth > positionDepth.Depth)
                .ToList();

            return targetList.Select(x =>
            {
                return new PlayerResponse
                {
                    PlayerId = x.PlayerId,
                    PlayerName = x.PlayerName
                };
            }).ToList();
        }
    }
}
