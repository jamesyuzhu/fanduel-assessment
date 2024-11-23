using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Models;
using DepthChart.Api.Repositories;
using DepthChart.Api.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DepthChart.Api.UnitTests.Services
{
    public class NFLDepthChartServiceRemovePlayerTests
    {
        private readonly DepthChartDbContext _context;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";

        public NFLDepthChartServiceRemovePlayerTests()
        {
            // Set up an in-memory database for testing
            var options = new DbContextOptionsBuilder<DepthChartDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Each test gets a unique in-memory database
                .Options;

            _context = new DepthChartDbContext(options);

            // Initialize the service with the in-memory context
            _service = new NFLDepthChartService(_context);
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(null, TeamCodeA));
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_ShouldThrowArgumentNullException_WhenTeamCodeIsNull() 
        {
            // Arrange
            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 0,
                PositionCode = "LT"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(request, null));
        }


        [Fact]
        public async Task RemovePlayerFromDepthChart_NullPositionCode_ShouldThrowException() 
        {
            // Arrange
            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 0                 
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_PlayerNotExists_ShouldReturnNull()
        {
            // Arrange
            var positionCode = "LT";
            await CreatePositionDepthRecordAsync(positionCode, 1, 1);
            await CreatePositionDepthRecordAsync(positionCode, 2, 2);
            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 3,
                PositionCode = "LT"
            };

            // Act & Assert
            var response = await _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA);
            Assert.Null(response.PlayerId);
            Assert.Null(response.PlayerName);
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_PlayerExists_ShouldRemoveAndAdjustDepth()
        {
            // Arrange
            var positionCode = "LT";
            await CreatePositionDepthRecordAsync(positionCode, 1, 1);
            await CreatePositionDepthRecordAsync(positionCode, 2, 2);
            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 1,
                PositionCode = "LT"
            };

            // Act & Assert
            var response = await _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA);
            Assert.Equal(1, response.PlayerId);
            var today = DateTime.Today;
            var weekStartDay = today.AddDays(-(int)today.DayOfWeek);
            var record =  await _context.ChartPositionDepths.FirstOrDefaultAsync(x => x.SportCode == _service.SportCode && x.TeamCode == TeamCodeA && x.WeekStartDate == weekStartDay && x.PositionCode == positionCode && x.PlayerId == 2);
            Assert.Equal(1, record.Depth);
        }

        private async Task<ChartPositionDepth> CreatePositionDepthRecordAsync(string positionCode, int playerId, int depth)
        {
            var today = DateTime.Today;

            var record = _context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                PositionCode = positionCode,
                PlayerId = playerId,
                PlayerName = "Player2",
                Depth = depth,
                WeekStartDate = today.AddDays(-(int)today.DayOfWeek),
                TeamCode = TeamCodeA,
                SportCode = _service.SportCode
            });
            await _context.SaveChangesAsync();
            return record.Entity;
        }

        private async Task<ChartPositionDepth> GetPositionDepthRecordAsync(string sportCode, string teamCode, DateTime weekStartDate, string positionCode, int playId)
        {
            return await _context.ChartPositionDepths.FirstOrDefaultAsync(x => x.SportCode == sportCode && x.TeamCode == teamCode && x.WeekStartDate == weekStartDate && x.PositionCode == positionCode && x.PlayerId == playId);
        }
    }
}
