using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Exceptions;
using DepthChart.Api.Models;
using DepthChart.Api.Repositories;
using DepthChart.Api.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DepthChart.Api.UnitTests.Services
{
    public class NFLDepthChartServiceRemovePlayerTests
    {
        private readonly Mock<IDepthChartRepository> _mockRepository;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";

        public NFLDepthChartServiceRemovePlayerTests()
        {
            _mockRepository = new Mock<IDepthChartRepository>();
            _service = new NFLDepthChartService(_mockRepository.Object);
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
                PlayerId = 1,
                PositionCode = "LT"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(request, null));
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_ShouldThrowException_WhenPositionCodeIsNull()
        {
            // Arrange
            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_ShouldThrowPlayerNotInPositionException_WhenPlayerNotExists()
        {
            // Arrange
            var positionCode = "QB";
            var weekStartDate = DateTime.UtcNow.Date;
            var positionDepthList = new List<ChartPositionDepth>
            {
                new ChartPositionDepth { PlayerId = 1, PlayerName = "John Doe", Depth = 1, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 2, PlayerName = "Jack Smith", Depth = 2, PositionCode = positionCode }
            };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, TeamCodeA, weekStartDate))
                .ReturnsAsync(positionDepthList);
            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 3,
                PositionCode = positionCode
            };

            // Act & Assert             
            await Assert.ThrowsAsync<PlayerNotInPositionException>(() => _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA, weekStartDate));
        }

        [Fact]
        public async Task RemovePlayerFromDepthChart_ShouldRemoveAndAdjustDepth_WhenSubsequentPlayersExist()
        {
            // Arrange
            var positionCode = "QB";
            var weekStartDate = DateTime.UtcNow.Date;
            var positionDepthList = new List<ChartPositionDepth>
            {
                new ChartPositionDepth { PlayerId = 1, PlayerName = "John Doe", Depth = 1, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 2, PlayerName = "Jack Smith", Depth = 2, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 3, PlayerName = "William Fang", Depth = 3, PositionCode = positionCode }
            };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, TeamCodeA, weekStartDate))
                .ReturnsAsync(positionDepthList);

            var request = new RemovePlayerFromDepthChartRequest
            {
                PlayerId = 2,
                PositionCode = positionCode
            };

            // Act
            var result = await _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.PlayerId);
            Assert.Equal("Jack Smith", result.PlayerName);             
            Assert.Equal(2, positionDepthList[1].Depth);             

            _mockRepository.Verify(r => r.RemovePlayerFromDepthChartAsync(
                It.Is<ChartPositionDepth>(p => p.PlayerId == 2),
                It.Is<List<ChartPositionDepth>>(list => list.Count == 1 && list[0].PlayerId == 3 && list[0].Depth == 2)),
                Times.Once);
        }

        [Fact]
        public async Task RemovePlayerFromDepthChartAsync_ShouldNotUpdateDepth_WhenNoSubsequentPlayersExist()
        {
            // Arrange            
            var positionCode = "QB";             
            var weekStartDate = DateTime.UtcNow.Date;

            var positionDepthList = new List<ChartPositionDepth>
            {
                new ChartPositionDepth { PlayerId = 1, PlayerName = "John Doe", Depth = 1, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 2, PlayerName = "Jack Smith", Depth = 2, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 3, PlayerName = "William Fang", Depth = 3, PositionCode = positionCode }
            };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, TeamCodeA, weekStartDate))
                .ReturnsAsync(positionDepthList);

            var request = new RemovePlayerFromDepthChartRequest
            {
                PositionCode = positionCode,
                PlayerId = 3
            };

            // Act
            var result = await _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.PlayerId);
            Assert.Equal("William Fang", result.PlayerName);             

            _mockRepository.Verify(r => r.RemovePlayerFromDepthChartAsync(
                It.Is<ChartPositionDepth>(p => p.PlayerId == 3),
                It.Is<List<ChartPositionDepth>>(list => list.Count == 0)),
                Times.Once);
        }
    }
}
