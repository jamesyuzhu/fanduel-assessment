using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Models;
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
        /// <param name="teamCode">The target team</param>
        /// <returns>The new created ChartPositionDepth record</returns>
        Task<AddPlayerToDepthChartResponse> AddPlayerToDepthChart(AddPlayerToDepthChartRequest request, string teamCode);

        //// Remove player from the DepthChart and return the player data
        //Task<(int playerId, string playerName)> RemovePlayerFromDepthChart(string positionCode, int playerId, string teamCode);

        //Task<List<(int playerId, string playerName)>> getBackups(string positionCode, int playerId, string teamCode, string sportCode)

        //Task<List<(int playerId, string playerName)>> getFullDepthChart(string positionCode, int playerId, string teamCode, string sportCode)
    }
}
