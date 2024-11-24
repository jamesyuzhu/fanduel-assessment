using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Models;
using DepthChart.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace DepthChart.Api.IntegrationTests
{
    [TestClass]
    public class DepthChartApiAddPlayerTests
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private const string SportCode = "NFL";
        private const string TeamCode = "TampaBayBuccaneers";
        private const string Url = $"/api/depthchart/{SportCode}/{TeamCode}";
        private readonly Util _util;

        public DepthChartApiAddPlayerTests()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            _util = new Util(SportCode, TeamCode);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PositionIsNull_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var request = new
            {
                PlayerId = 1,
                PlayerName = "John Smith",                 
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerIdNull_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var request = new
            {                 
                PlayerName = "John Smith",
                PositionCode = "QB",
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_InvalidPlayerId_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var request = new
            {
                PlayerId = 0,
                PlayerName = "John Smith",
                PositionCode = "QB",
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerNameNull_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var request = new
            {
                PlayerId = 1,                
                PositionCode = "QB",
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_InvalidDepth_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var request = new
            {
                PlayerId = 1,
                PlayerName = "John Smith",
                PositionCode = "QB",
                Depth = 0
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_EmptyDepthChart_ShouldAddPlayerCorrectly()
        {
            // Arrange
            var request = new
            {
                PlayerId = 1,
                PlayerName = "John Smith",
                PositionCode = "QB",
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthIndexGreaterThanCurrentPositionDepthLength_ShouldAddPlayerAtTheEnd()
        {
            var positionCode = "LT";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();
            // Access the service provider
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 100, 1, context, null, "LT Player1");
             
            // Arrange
            var request = new
            {
                PlayerId = 101,
                PlayerName = "LT Player2",
                PositionCode = positionCode,
                Depth = 3
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<AddPlayerToDepthChartResponse>(response);
            responseData.Should().NotBeNull();
            responseData?.PlayerId.Should().Be(101);
            responseData?.Depth.Should().Be(2);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthIndexEqualCurrentPositionDepthLength_ShouldAddPlayerAtGivenDepthAndShiftTheOthers()
        {
            // Arrange
            var positionCode = "ED";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();
            
            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 1, context, null, "Susan Lee");             
            
            var request = new
            {
                PlayerId = 1,
                PlayerName = "John Smith",
                PositionCode = positionCode,
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<AddPlayerToDepthChartResponse>(response);            
            responseData.Should().NotBeNull();
            responseData?.PlayerId.Should().Be(1);
            responseData?.Depth.Should().Be(1);
            var existingPlayer = await _util.GetExistingPlayer(positionCode, weekStartDate, 2, context);             
            existingPlayer?.Depth.Should().Be(2);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthIndexInMiddle_ShouldAddPlayerAtGivenDepthAndShiftSuccessors()
        {
            // Arrange
            var positionCode = "MD";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, null, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, null, "Jessy Wu");
            await _util.CreateChartPositionDepthRecord(positionCode, 3, 3, context, null, "Mark Fang");           
            await context.SaveChangesAsync();

            var request = new
            {
                PlayerId = 4,
                PlayerName = "John Smith",
                PositionCode = positionCode,
                Depth = 2
            };

            // Act
            var response = await _client.PostAsJsonAsync(Url, request);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<AddPlayerToDepthChartResponse>(response);
            responseData.Should().NotBeNull();
            responseData?.PlayerId.Should().Be(4);
            responseData?.Depth.Should().Be(2);
            var existingPlayer2 = await _util.GetExistingPlayer(positionCode, weekStartDate, 2, context);
            existingPlayer2?.Depth.Should().Be(3);
            var existingPlayer3 = await _util.GetExistingPlayer(positionCode, weekStartDate, 3, context);
            existingPlayer3?.Depth.Should().Be(4);
            var existingPlayer1 = await _util.GetExistingPlayer(positionCode, weekStartDate, 1, context);
            existingPlayer1?.Depth.Should().Be(1);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthIndexInMiddleAndChartDateIsGiven_ShouldAddPlayerAtGivenDepthAndShiftSuccessors()
        {
            // Arrange
            var positionCode = "MDC";
            var chartDate = DateTime.UtcNow.Date.AddDays(-7);

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, chartDate, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, chartDate, "Jessy Wu");
            await _util.CreateChartPositionDepthRecord(positionCode, 3, 3, context, chartDate, "Mark Fang");
            await context.SaveChangesAsync();

            var request = new
            {
                PlayerId = 4,
                PlayerName = "John Smith",
                PositionCode = positionCode,
                Depth = 2,
                ChartDate = chartDate
            };

            // Act
            var response = await _client.PostAsJsonAsync($"{Url}?chartDate={chartDate.ToString("yyyy-MM-dd")}", request);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<AddPlayerToDepthChartResponse>(response);
            responseData.Should().NotBeNull();
            responseData?.PlayerId.Should().Be(4);
            responseData?.Depth.Should().Be(2);
            var existingPlayer2 = await _util.GetExistingPlayer(positionCode, chartDate, 2, context);
            existingPlayer2?.Depth.Should().Be(3);
            var existingPlayer3 = await _util.GetExistingPlayer(positionCode, chartDate, 3, context);
            existingPlayer3?.Depth.Should().Be(4);
            var existingPlayer1 = await _util.GetExistingPlayer(positionCode, chartDate, 1, context);
            existingPlayer1?.Depth.Should().Be(1);
        }
    }
}