using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Exceptions;
using DepthChart.Api.Repositories;
using DepthChart.Api.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DepthChart.Api.UnitTests.Services
{
    public class NFLDepthChartServiceGetBackUpsTests
    {
        private readonly DepthChartDbContext _context;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";
        private readonly DataUtil _util;

        public NFLDepthChartServiceGetBackUpsTests()
        {
            // Set up an in-memory database for testing
            var options = new DbContextOptionsBuilder<DepthChartDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Each test gets a unique in-memory database
                .Options;

            _context = new DepthChartDbContext(options);

            // Initialize the service with the in-memory context
            _service = new NFLDepthChartService(_context);

            // Set up the DataUtil instance
            _util = new DataUtil(_service.SportCode, TeamCodeA, _context);
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
            var positionCode = "GBU1";
            await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1);
            await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2);
            var request = new GetBackUpsRequest
            {
                PlayerId = 3,
                PositionCode = positionCode
            };
             
            // Act & Assert
            await Assert.ThrowsAsync<PlayerNotInPositionException>(() => _service.GetBackupsAsync(request, TeamCodeA));
        }

        [Fact]
        public async Task GetBackUps_ShouldReturnEmptyList_WhenPlayerHasNoSuccessor()
        {
            // Arrange
            var positionCode = "GBU2";
            await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1);
            await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2);
            var request = new GetBackUpsRequest
            {
                PlayerId = 2,
                PositionCode = positionCode
            };

            // Act & Assert
            var response = await _service.GetBackupsAsync(request, TeamCodeA);
            Assert.Equal(0, response?.Count);            
        }

        [Fact]
        public async Task GetBackUps_ShouldReturnSuccessorList_WhenPlayerHasSuccessor()
        {
            // Arrange
            var positionCode = "GBU3";
            await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1);
            await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2);
            await _util.CreatePositionDepthRecordAsync(positionCode, 3, 3);
            var request = new GetBackUpsRequest
            {
                PlayerId = 1,
                PositionCode = positionCode
            };

            // Act & Assert
            var response = await _service.GetBackupsAsync(request, TeamCodeA);
            Assert.Equal(2, response?.Count);
            Assert.Equal(2, response[0].PlayerId);
            Assert.Equal(3, response[1].PlayerId);
        }             
    }
}
