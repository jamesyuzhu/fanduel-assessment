using DepthChart.Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DepthChart.Api.Repositories
{
    public class DepthChartEFRepository : IDepthChartRepository
    {
        private readonly DepthChartDbContext _context;

        public DepthChartEFRepository(DepthChartDbContext context)
        {
            _context = context;
        }

        public async Task AddPlayerToDepthChartAsync(ChartPositionDepth target, List<ChartPositionDepth> successors)
        {
            // Add the target record
            _context.ChartPositionDepths.Add(target);

            // Update the successor records
            if (successors != null && successors.Count > 0)
            {
                _context.ChartPositionDepths.UpdateRange(successors);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ChartPositionDepth>> GetFullDepthChartAsync(string sportCode, string teamCode, DateTime chartDate)
        {
            return await _context.ChartPositionDepths
                           .Where(x => x.SportCode.ToLowerInvariant() == sportCode.ToLowerInvariant()
                                    && x.TeamCode.ToLowerInvariant() == teamCode.ToLowerInvariant()
                                    && x.ChartDate == chartDate)
                           .ToListAsync();
        }

        public async Task RemovePlayerFromDepthChartAsync(ChartPositionDepth target, List<ChartPositionDepth> successors)
        {
            // Remove the target record
            _context.ChartPositionDepths.Remove(target);

            // Update the successor records
            if (successors != null && successors.Count > 0)
            {
                _context.ChartPositionDepths.UpdateRange(successors);
            }
            
            await _context.SaveChangesAsync();
        }
    }
}
