﻿using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DepthChart.Api.Services.Interface
{
    public interface IDepthChartService
    {
        // The sport code like NBA, NFL 
        string SportCode { get; }
        
        // Same sport service can support multiple teams
        List<string> TeamCodes { get; }

        /// <summary>
        /// Add players at a position with the given depth to a specified DepthChart
        /// </summary>
        /// <param name="request">The given request</param>
        /// <param name="teamCode">The code of the target team</param>
        /// <returns>The new created ChartPositionDepth record</returns>
        Task<AddPlayerToDepthChartResponse> AddPlayerToDepthChartAsync(AddPlayerToDepthChartRequest request, string teamCode);

        /// <summary>
        /// Remove a player from the DepthChart at the given position
        /// </summary>
        /// <param name="request">The given request</param>
        /// <param name="teamCode">The code of the target team</param>
        /// <returns>The remove player record<returns>
        Task<PlayerResponse> RemovePlayerFromDepthChartAsync(RemovePlayerFromDepthChartRequest request, string teamCode);

        /// <summary>
        /// For a given player and position, we want to see all players that are “Backups”, those with a lower position_depth 
        /// </summary>
        /// <param name="request">The given request</param>
        /// <param name="teamCode">The code of the target team</param>
        /// <returns></returns>
        Task<List<PlayerResponse>> GetBackupsAsync(GetBackUpsRequest request, string teamCode);

        //Task<List<(int playerId, string playerName)>> getFullDepthChart(string positionCode, int playerId, string teamCode, string sportCode)
    }
}
