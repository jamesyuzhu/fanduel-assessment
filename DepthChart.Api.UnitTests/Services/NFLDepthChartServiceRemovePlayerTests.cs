using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Exceptions;
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
        //private readonly DepthChartDbContext _context;
        //private readonly NFLDepthChartService _service;
        //private const string TeamCodeA = "TeamCodeA";
        //private readonly DataUtil _util;

        //public NFLDepthChartServiceRemovePlayerTests()
        //{
        //    // Set up an in-memory database for testing
        //    var options = new DbContextOptionsBuilder<DepthChartDbContext>()
        //        .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Each test gets a unique in-memory database
        //        .Options;

        //    _context = new DepthChartDbContext(options);

        //    // Initialize the service with the in-memory context
        //    _service = new NFLDepthChartService(_context);

        //    // Set up the DataUtil instance
        //    _util = new DataUtil(_service.SportCode, TeamCodeA, _context);
        //}

        //[Fact]
        //public async Task RemovePlayerFromDepthChart_ShouldThrowArgumentNullException_WhenRequestIsNull()
        //{
        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(null, TeamCodeA));
        //}

        //[Fact]
        //public async Task RemovePlayerFromDepthChart_ShouldThrowArgumentNullException_WhenTeamCodeIsNull() 
        //{
        //    // Arrange
        //    var request = new RemovePlayerFromDepthChartRequest
        //    {
        //        PlayerId = 1,
        //        PositionCode = "LT"
        //    };

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(request, null));
        //}


        //[Fact]
        //public async Task RemovePlayerFromDepthChart_ShouldThrowException_WhenPositionCodeIsNull() 
        //{
        //    // Arrange
        //    var request = new RemovePlayerFromDepthChartRequest
        //    {
        //        PlayerId = 1                 
        //    };

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA));
        //}

        //[Fact]
        //public async Task RemovePlayerFromDepthChart_ShouldThrowPlayerNotInPositionException_WhenPlayerNotExists()
        //{
        //    // Arrange
        //    var positionCode = "LT";
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1);
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2);
        //    var request = new RemovePlayerFromDepthChartRequest
        //    {
        //        PlayerId = 3,
        //        PositionCode = positionCode
        //    };

        //    // Act & Assert             
        //    await Assert.ThrowsAsync<PlayerNotInPositionException>(() => _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA));
        //}

        //[Fact]
        //public async Task RemovePlayerFromDepthChart_ShouldRemoveAndAdjustDepth_WhenPlayerExists()
        //{
        //    // Arrange
        //    var positionCode = "LT";
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1);
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2);
        //    var request = new RemovePlayerFromDepthChartRequest
        //    {
        //        PlayerId = 1,
        //        PositionCode = positionCode
        //    };

        //    // Act & Assert
        //    var response = await _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA);
        //    Assert.Equal(1, response.PlayerId);
        //    var today = DateTime.Today;
        //    var weekStartDay = today.AddDays(-(int)today.DayOfWeek);
        //    var record =  await _context.ChartPositionDepths.FirstOrDefaultAsync(x => x.SportCode == _service.SportCode && x.TeamCode == TeamCodeA && x.ChartDate == weekStartDay && x.PositionCode == positionCode && x.PlayerId == 2);
        //    Assert.Equal(1, record.Depth);
        //}

        //[Fact]
        //public async Task RemovePlayerFromDepthChart_ShouldRemoveAndAdjustDepth_WhenPlayerExistsAndChartDateIsGiven()
        //{
        //    // Arrange
        //    var positionCode = "LTC";
        //    var chartDate = DateTime.Today.AddDays(-7);
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 1, 1, chartDate);
        //    await _util.CreatePositionDepthRecordAsync(positionCode, 2, 2, chartDate);
        //    var request = new RemovePlayerFromDepthChartRequest
        //    {
        //        PlayerId = 1,
        //        PositionCode = positionCode
        //    };

        //    // Act & Assert
        //    var response = await _service.RemovePlayerFromDepthChartAsync(request, TeamCodeA, chartDate);
        //    Assert.Equal(1, response.PlayerId);            
        //    var record = await _context.ChartPositionDepths.FirstOrDefaultAsync(x => x.SportCode == _service.SportCode && x.TeamCode == TeamCodeA && x.ChartDate == chartDate && x.PositionCode == positionCode && x.PlayerId == 2);
        //    Assert.Equal(1, record.Depth);
        //}
    }
}
