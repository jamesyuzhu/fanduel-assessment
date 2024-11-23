using DepthChart.Api.Models;
using DepthChart.Api.Repositories;
using System;
using System.Threading.Tasks;

namespace DepthChart.Api.UnitTests
{
    public class DataUtil
    {
        private readonly string _sportCode;
        private readonly string _teamCode;
        private readonly DepthChartDbContext _context;

        public DataUtil(string sportCode, string teamCode, DepthChartDbContext context)
        {
            _sportCode = sportCode;
            _teamCode = teamCode;
            _context = context;
        }
        public async Task<ChartPositionDepth> CreatePositionDepthRecordAsync(string positionCode, int playerId, int depth, DateTime? chartDate = null, string playerName = "Tester")
        {
            var today = DateTime.Today;
            var weekStartDate = chartDate ?? today.AddDays(-(int)today.DayOfWeek);

            var record = _context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                PositionCode = positionCode,
                PlayerId = playerId,
                PlayerName = playerName,
                Depth = depth,
                WeekStartDate = weekStartDate,
                TeamCode = _teamCode,
                SportCode = _sportCode
            });
            await _context.SaveChangesAsync();
            return record.Entity;
        }
    }
}
