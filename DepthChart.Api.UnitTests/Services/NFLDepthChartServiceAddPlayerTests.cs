using System;
using System.Threading.Tasks;
using DepthChart.Api.Services;
using DepthChart.Api.Repositories; 
using Xunit;
using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Models;
using Moq;
using System.Collections.Generic;

namespace DepthChart.Api.UnitTests.Services
{
    public class NFLDepthChartServiceAddPlayerTests
    {
        private readonly Mock<IDepthChartRepository> _mockRepository;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";

        public NFLDepthChartServiceAddPlayerTests()
        {
            _mockRepository = new Mock<IDepthChartRepository>();
            _service = new NFLDepthChartService(_mockRepository.Object);
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
        public async Task AddPlayerToDepthChart_ShouldThrowArgumentNullException_WhenPositionCodeIsNull()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                Depth = 1,
                PlayerName = "Player1"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddPlayerToDepthChartAsync(request, TeamCodeA));
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
        public async Task AddPlayerToDepthChartAsync_ShouldAddPlayerToEndOfDepthChart_WhenDepthIsNull()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 1,
                PlayerName = "John Doe",
                PositionCode = "QB"
            };

            var teamCode = "TeamA";
            var weekStartDate = DateTime.UtcNow.Date;
            var positionDepthList = new List<ChartPositionDepth>();

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, teamCode, weekStartDate))
                .ReturnsAsync(positionDepthList);

            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, teamCode, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.PlayerId);
            Assert.Equal("John Doe", result.PlayerName);
            Assert.Equal(1, result.Depth);           

            _mockRepository.Verify(r => r.AddPlayerToDepthChartAsync(
                It.Is<ChartPositionDepth>(p => p.PlayerId == 1 && p.Depth == 1),
                It.IsAny<List<ChartPositionDepth>>()), Times.Once);
        }

        [Fact]
        public async Task AddPlayerToDepthChartAsync_ShouldReorderDepthChart_WhenDepthIsSpecified()
        {
            // Arrange
            var request = new AddPlayerToDepthChartRequest
            {
                PlayerId = 2,
                PlayerName = "Jane Doe",
                PositionCode = "QB",
                Depth = 2
            };

            var teamCode = "TeamA";
            var weekStartDate = DateTime.UtcNow.Date;

            var positionDepthList = new List<ChartPositionDepth>
            {
                new ChartPositionDepth { PlayerId = 1, PlayerName = "John Doe", Depth = 1, PositionCode = "QB" },
                new ChartPositionDepth { PlayerId = 3, PlayerName = "Jack Smith", Depth = 2, PositionCode = "QB" }
            };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, teamCode, weekStartDate))
                .ReturnsAsync(positionDepthList);

            // Act
            var result = await _service.AddPlayerToDepthChartAsync(request, teamCode, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PlayerId);
            Assert.Equal("Jane Doe", result.PlayerName);
            Assert.Equal(2, result.Depth);
            Assert.Equal(3, positionDepthList[1].Depth);

            _mockRepository.Verify(r => r.AddPlayerToDepthChartAsync(
                It.Is<ChartPositionDepth>(p => p.PlayerId == 2 && p.Depth == 2),
                It.Is<List<ChartPositionDepth>>(list => list.Count == 1 && list[0].PlayerId == 3 && list[0].Depth == 3)),
                Times.Once);
        }         
    }
}
