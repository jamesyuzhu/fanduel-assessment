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
    public class NFLDepthChartServiceGetFullDepthChartTests
    {
        private readonly Mock<IDepthChartRepository> _mockRepository;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";
        private const string PositionCodeQB = "QB";

        public NFLDepthChartServiceGetFullDepthChartTests()
        {
            _mockRepository = new Mock<IDepthChartRepository>();
            _service = new NFLDepthChartService(_mockRepository.Object);
        }
        
        [Fact]
        public async Task GetFullDepthChart_ShouldThrowArgumentNullException_WhenTeamCodeIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetFullDepthChart(null));
        }

        [Fact]
        public async Task GetFullDepthChart_ShouldReturnFullList_WhenThereAreData()
        {
            // Arrange            
            var weekStartDate = DateTime.UtcNow.Date;

            var positionDepthList = new List<ChartPositionDepth>
        {
            new ChartPositionDepth { PositionCode = PositionCodeQB, PlayerId = 1, PlayerName = "John Doe", Depth = 1 },
            new ChartPositionDepth { PositionCode = PositionCodeQB, PlayerId = 2, PlayerName = "Jane Smith", Depth = 2 },
            new ChartPositionDepth { PositionCode = "RB", PlayerId = 3, PlayerName = "Emily Davis", Depth = 1 }
        };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(It.IsAny<string>(), TeamCodeA, weekStartDate))
                .ReturnsAsync(positionDepthList);

            // Act
            var result = await _service.GetFullDepthChart(TeamCodeA, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);

            // Validate individual items
            Assert.Equal(PositionCodeQB, result[0].PositionCode);
            Assert.Equal(1, result[0].PlayerId);
            Assert.Equal("John Doe", result[0].PlayerName);
            Assert.Equal(1, result[0].Depth);

            Assert.Equal(PositionCodeQB, result[1].PositionCode);
            Assert.Equal(2, result[1].PlayerId);
            Assert.Equal("Jane Smith", result[1].PlayerName);
            Assert.Equal(2, result[1].Depth);

            Assert.Equal("RB", result[2].PositionCode);
            Assert.Equal(3, result[2].PlayerId);
            Assert.Equal("Emily Davis", result[2].PlayerName);
            Assert.Equal(1, result[2].Depth);
        }

        [Fact]
        public async Task GetFullDepthChart_ShouldReturnEmptyList_WhenNoData()
        {
            // Arrange             
            var weekStartDate = DateTime.UtcNow.Date;

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(It.IsAny<string>(), TeamCodeA, weekStartDate))
                .ReturnsAsync(new List<ChartPositionDepth>());

            // Act
            var result = await _service.GetFullDepthChart(TeamCodeA, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFullDepthChart_ShouldUseDefaultChartDate_WhenChartDateIsNull()
        {
            // Arrange             
            var defaultWeekStartDate = DateTime.UtcNow.Date;

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(It.IsAny<string>(), TeamCodeA, defaultWeekStartDate))
                .ReturnsAsync(new List<ChartPositionDepth>());

            // Act
            var result = await _service.GetFullDepthChart(TeamCodeA);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            // Verify repository call
            _mockRepository.Verify(r => r.GetFullDepthChartAsync(It.IsAny<string>(), TeamCodeA, defaultWeekStartDate), Times.Once);
        }
    }
}
