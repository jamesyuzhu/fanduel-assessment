using System;
using System.Threading.Tasks;
using DepthChart.Api.Services;
using DepthChart.Api.Repositories;
using Microsoft.EntityFrameworkCore; 
using Xunit;
using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Models;

namespace DepthChart.Api.UnitTests.Services
{
    public class NFLDepthChartServiceAddPlayerTests
    {
        private readonly DepthChartDbContext _context;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";

        public NFLDepthChartServiceAddPlayerTests()
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
        public async Task AddPlayerToDepthChart_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddPlayerToDepthChartAsync(null, TeamCodeA));
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldThrowArgumentNullException_WhenTeamCodeIsNull()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 0,
                Depth = 1,
                PositionCode = "QB",
                PlayerName = "Player1"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddPlayerToDepthChartAsync(request, null));
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldThrowInvalidOperationExceptionException_WhenPositionCodeIsNull()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 1,                 
                PlayerName = "Player1"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddPlayerToDepthChartAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldThrowArgumentNullException_WhenPlayerNameIsNull()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 1,
                PositionCode = "QB",                
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddPlayerToDepthChartAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldThrowArgumentOutOfRangeException_WhenPlayerIdIsLessThan1()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 0,
                Depth = 1,
                PositionCode = "QB",                 
                PlayerName = "Player1"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.AddPlayerToDepthChartAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldThrowArgumentOutOfRangeException_WhenDepthIsLessThan1()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 0,
                PositionCode = "QB",                 
                PlayerName = "Player1"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _service.AddPlayerToDepthChartAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldCreateNewDepthChart_WhenNoExistingDepthChartFound()
        {
            // Arrange            
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 1,
                PositionCode = "QB",               
                PlayerName = "Player1"
            };
            
            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, TeamCodeA);

            // Assert            
            Assert.Equal(TeamCodeA, result.TeamCode);
        }        

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldAddNewDepth_WhenDepthDoesNotExist()
        {
            // Arrange             
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 2,
                PositionCode = "QB",                
                PlayerName = "Player1"
            };                  

            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, TeamCodeA);

            // Assert            
            Assert.Equal(request.PlayerId, result.PlayId);
            Assert.Equal(1, result.Depth);
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldAddToEnd_WhenDepthIsNotGivenAndPositionChartIsEmpty()
        {
            // Arrange             
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                PositionCode = "QB",
                PlayerName = "Player1"
            };           

            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, TeamCodeA);

            // Assert            
            Assert.Equal(request.PlayerId, result.PlayId);
            Assert.Equal(1, result.Depth);
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldAddToEnd_WhenDepthIsNotGivenAndPositionChartIsNotEmpty()
        {
            // Arrange
            var positionCode = "QB";
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                PositionCode = positionCode,
                PlayerName = "Player1"
            };

            var existingRecord = await CreateOnePositionDepthRecordAsync(positionCode);

            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, TeamCodeA);

            // Assert            
            Assert.Equal(request.PlayerId, result.PlayId);
            Assert.Equal(2, result.Depth);            
        }

        [Fact]
        public async Task AddPlayerToDepthChart_ShouldBeInsertAtGivenDepth_WhenDepthIsGivenAndPositionChartIsNotEmpty()
        {
            // Arrange
            var positionCode = "QB";
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 1,
                PositionCode = "QB",
                PlayerName = "Player1"
            };

            var existingRecord = await CreateOnePositionDepthRecordAsync(positionCode);

            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, TeamCodeA);

            // Assert            
            Assert.Equal(request.PlayerId, result.PlayId);
            Assert.Equal(1, result.Depth);
            Assert.Equal(2, existingRecord.Depth);
        }

        private async Task<ChartPositionDepth> CreateOnePositionDepthRecordAsync(string positionCode)
        {
            var today = DateTime.Today;
             
            var record = _context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                PositionCode = positionCode,
                PlayerId = 2,
                PlayerName = "Player2",
                Depth = 1,
                WeekStartDate = today.AddDays(-(int)today.DayOfWeek),
                TeamCode = TeamCodeA,
                SportCode = _service.SportCode
            });
            await _context.SaveChangesAsync();
            return record.Entity;
        }

    }
}
