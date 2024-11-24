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
    public class NFLDepthChartServiceGetBackUpsTests
    {
        private readonly Mock<IDepthChartRepository> _mockRepository;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";

        public NFLDepthChartServiceGetBackUpsTests()
        {
            _mockRepository = new Mock<IDepthChartRepository>();
            _service = new NFLDepthChartService(_mockRepository.Object);
        }         

        [Fact]
        public async Task GetBackUps_ShouldThrowArgumentNullException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetBackupsAsync(null, TeamCodeA));
        }

        [Fact]
        public async Task GetBackUps_ShouldThrowArgumentNullException_WhenTeamCodeIsNull()
        {
            // Arrange
            var request = new GetBackUpsRequest
            {
                PlayerId = 1,
                PositionCode = "GBU"
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetBackupsAsync(request, null));
        }

        [Fact]
        public async Task GetBackUps_ShouldThrowException_WhenPositionCodeIsNull()
        {
            // Arrange
            var request = new GetBackUpsRequest
            {
                PlayerId = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetBackupsAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task GetBackUps_ShouldThrowPlayerNotInPositionException_WhenPlayerNotExists()
        {
            // Arrange
            var request = new GetBackUpsRequest
            {
                PositionCode = "QB",
                PlayerId = 2
            };

            var teamCode = "TeamA";
            var weekStartDate = DateTime.UtcNow.Date;
            var positionDepthList = new List<ChartPositionDepth>();

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, teamCode, weekStartDate))
                .ReturnsAsync(positionDepthList);

            // Act & Assert
            await Assert.ThrowsAsync<PlayerNotInPositionException>(() =>
                _service.GetBackupsAsync(request, teamCode, weekStartDate));
        }

        [Fact]
        public async Task GetBackUps_ShouldReturnEmptyList_WhenPlayerHasNoSuccessor()
        {
            // Arrange
            var positionCode = "QB";
            var request = new GetBackUpsRequest
            {
                PositionCode = positionCode,
                PlayerId = 4
            };

            var teamCode = "TeamA";
            var weekStartDate = DateTime.UtcNow.Date;

            var positionDepthList = new List<ChartPositionDepth>
        {
            new ChartPositionDepth { PlayerId = 1, PlayerName = "John Doe", Depth = 1, PositionCode = positionCode },
            new ChartPositionDepth { PlayerId = 4, PlayerName = "Emily Davis", Depth = 2, PositionCode = positionCode }
        };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, teamCode, weekStartDate))
                .ReturnsAsync(positionDepthList);

            // Act
            var result = await _service.GetBackupsAsync(request, teamCode, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetBackUps_ShouldReturnSuccessorList_WhenPlayerHasSuccessor()
        {
            // Arrange
            var positionCode = "QB";            

            var teamCode = "TeamA";
            var weekStartDate = DateTime.UtcNow.Date;

            var positionDepthList = new List<ChartPositionDepth>
            {
                new ChartPositionDepth { PlayerId = 1, PlayerName = "John Doe", Depth = 1, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 2, PlayerName = "Jane Doe", Depth = 2, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 3, PlayerName = "Jack Smith", Depth = 3, PositionCode = positionCode },
                new ChartPositionDepth { PlayerId = 4, PlayerName = "Emily Davis", Depth = 4, PositionCode = positionCode }
            };

            _mockRepository.Setup(r => r.GetFullDepthChartAsync(_service.SportCode, teamCode, weekStartDate))
                .ReturnsAsync(positionDepthList);

            var request = new GetBackUpsRequest
            {
                PositionCode = "QB",
                PlayerId = 2
            };

            // Act
            var result = await _service.GetBackupsAsync(request, teamCode, weekStartDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(3, result[0].PlayerId);
            Assert.Equal("Jack Smith", result[0].PlayerName);

            Assert.Equal(4, result[1].PlayerId);
            Assert.Equal("Emily Davis", result[1].PlayerName);
        }        
    }
}
