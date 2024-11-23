using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DepthChart.Api.Models;

namespace DepthChart.Api.Repositories
{
    public interface IDepthChartRepository
    {
        /// <summary>
        /// Get all position depth (players) record of a specific DepthChart
        /// </summary>
        /// <param name="sportCode">The given sport code</param>
        /// <param name="teamCode">The given team code</param>
        /// <param name="chartDate">The given chart date</param>
        /// <returns>A full list of ChartPositionDepth records</returns>
        Task<List<ChartPositionDepth>> GetFullDepthChartAsync(string sportCode, string teamCode, DateTime chartDate);

        /// <summary>
        /// Add a ChartPositionDepth record to the repository along with all the updated successor records
        /// </summary>
        /// <param name="target">The new ChartPositionDepth record to be added</param>
        /// <param name="successors">All the successors records to be updated</param>
        /// <returns></returns>
        Task AddPlayerToDepthChartAsync(ChartPositionDepth target, List<ChartPositionDepth> successors);

        /// <summary>
        /// Remove a ChartPositionDepth record from the repository along with all the updated successor records
        /// </summary>
        /// <param name="target">The target ChartPositionDepth record to be deleted</param>
        /// <param name="successors">All the successors records to be updated</param>
        /// <returns></returns>
        Task RemovePlayerFromDepthChartAsync(ChartPositionDepth target, List<ChartPositionDepth> successors);         
    }
}
