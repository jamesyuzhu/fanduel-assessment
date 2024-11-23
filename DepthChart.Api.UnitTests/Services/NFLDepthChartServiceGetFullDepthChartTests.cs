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
    public class NFLDepthChartServiceGetFullDepthChartTests
    {
        private readonly DepthChartDbContext _context;
        private readonly NFLDepthChartService _service;
        private const string TeamCodeA = "TeamCodeA";
        private readonly DataUtil _util;

        public NFLDepthChartServiceGetFullDepthChartTests()
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
        public async Task GetFullDepthChart_ShouldThrowArgumentNullException_WhenTeamCodeIsNull()
        {             
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetFullDepthChart(null));
        }

        //[Fact]
        //public async Task GetBackUps_ShouldReturnEmptyList_WhenNoPositionAndPlayer()
        //{
        //    // Arrange
        //    var positionCode = "GBU2";
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1);
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2);
        //    var request = new GetBackUpsRequest
        //    {
        //        PlayerId = 2,
        //        PositionCode = positionCode
        //    };

        //    // Act & Assert
        //    var response = await _service.GetBackupsAsync(request, TeamCodeA);
        //    Assert.Equal(0, response?.Count);
        //}

        [Fact]
        public async Task GetFullDepthChart_ShouldReturnFullList_WhenThereAreData()
        {
            // Arrange
            var positionCode1 = "GF_1";
            var targetDate = DateTime.Today.AddDays(-7);
            await _util.CreatePositionDepthRecordAsync(positionCode1, 1, 1, targetDate);
            await _util.CreatePositionDepthRecordAsync(positionCode1, 2, 2, targetDate);
            await _util.CreatePositionDepthRecordAsync(positionCode1, 3, 3, targetDate);

            var positionCode2 = "GF_2";
            await _util.CreatePositionDepthRecordAsync(positionCode2, 4, 1, targetDate);
            await _util.CreatePositionDepthRecordAsync(positionCode2, 5, 2, targetDate);
            await _util.CreatePositionDepthRecordAsync(positionCode2, 6, 3, targetDate);
             
            // Act & Assert
            var response = await _service.GetFullDepthChart(TeamCodeA, targetDate);
            Assert.Equal(6, response?.Count);
        }

        [Fact]
        public async Task GetFullDepthChart_ShouldReturnEmptyList_WhenNoData()
        {
            // Arrange
            var positionCode1 = "GF_1";
            var targetDate = DateTime.Today.AddDays(-7);
            await _util.CreatePositionDepthRecordAsync(positionCode1, 1, 1);
            await _util.CreatePositionDepthRecordAsync(positionCode1, 2, 2);
            await _util.CreatePositionDepthRecordAsync(positionCode1, 3, 3);

            var positionCode2 = "GF_2";
            await _util.CreatePositionDepthRecordAsync(positionCode2, 4, 1);
            await _util.CreatePositionDepthRecordAsync(positionCode2, 5, 2);
            await _util.CreatePositionDepthRecordAsync(positionCode2, 6, 3);

            // Act & Assert
            var response = await _service.GetFullDepthChart(TeamCodeA, targetDate);
            Assert.Equal(0, response?.Count);
        }
    }
}
