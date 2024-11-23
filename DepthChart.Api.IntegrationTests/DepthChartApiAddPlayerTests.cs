using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Models;
using DepthChart.Api.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        public DepthChartApiAddPlayerTests()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
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
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

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
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

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
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

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
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

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
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthGreaterThanCurrentPositionDepthLength_ShouldAddPlayerAtTheEnd()
        {
            var positionCode = "LT";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();
            // Access the service provider
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                SportCode = SportCode,
                TeamCode = TeamCode,
                WeekStartDate = weekStartDate,
                PlayerId = 2,
                PlayerName = "Susan Lee",
                PositionCode = positionCode,
                Depth = 1
            });
            await context.SaveChangesAsync();

            // Arrange
            var request = new
            {
                PlayerId = 1,
                PlayerName = "John Smith",
                PositionCode = "LT",
                Depth = 3
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

            // Assert
            response.EnsureSuccessStatusCode();
             
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<AddPlayerToDepthChartResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
             
            responseData.Should().NotBeNull();
            responseData?.PlayId.Should().Be(1);
            responseData?.Depth.Should().Be(2);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthIndexEqualCurrentPositionDepthLength_ShouldAddPlayerAtGivenDepthAndShiftTheOthers()
        {
            // Arrange
            var positionCode = "LT";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();
            
            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                SportCode = SportCode,
                TeamCode = TeamCode,
                WeekStartDate = weekStartDate,
                PlayerId = 2,
                PlayerName = "Susan Lee",
                PositionCode = positionCode,
                Depth = 1
            });
            await context.SaveChangesAsync();
            
            var request = new
            {
                PlayerId = 1,
                PlayerName = "John Smith",
                PositionCode = positionCode,
                Depth = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await GetResponseData(response);            
            responseData.Should().NotBeNull();
            responseData?.PlayId.Should().Be(1);
            responseData?.Depth.Should().Be(1);
            var existingPlayer = await GetExistingPlayer(positionCode, weekStartDate, 2, context);             
            existingPlayer.Depth.Should().Be(2);
        }

        [TestMethod]
        public async Task AddPlayerToDepthChart_PlayerDepthIndexInMiddle_ShouldAddPlayerAtGivenDepthAndShiftTheOthers()
        {
            // Arrange
            var positionCode = "MD";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                SportCode = SportCode,
                TeamCode = TeamCode,
                WeekStartDate = weekStartDate,
                PlayerId = 1,
                PlayerName = "Susan Lee",
                PositionCode = positionCode,
                Depth = 1
            });
            context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                SportCode = SportCode,
                TeamCode = TeamCode,
                WeekStartDate = weekStartDate,
                PlayerId = 2,
                PlayerName = "Jessy Wu",
                PositionCode = positionCode,
                Depth = 2
            });
            context.ChartPositionDepths.Add(new Models.ChartPositionDepth
            {
                SportCode = SportCode,
                TeamCode = TeamCode,
                WeekStartDate = weekStartDate,
                PlayerId = 3,
                PlayerName = "Mark Fang",
                PositionCode = positionCode,
                Depth = 3
            });
            await context.SaveChangesAsync();

            var request = new
            {
                PlayerId = 4,
                PlayerName = "John Smith",
                PositionCode = positionCode,
                Depth = 2
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/depthchart/add-player-to-depth-chart/{SportCode}/{TeamCode}", request);

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await GetResponseData(response);
            responseData.Should().NotBeNull();
            responseData?.PlayId.Should().Be(4);
            responseData?.Depth.Should().Be(2);
            var existingPlayer2 = await GetExistingPlayer(positionCode, weekStartDate, 2, context);
            existingPlayer2.Depth.Should().Be(3);
            var existingPlayer3 = await GetExistingPlayer(positionCode, weekStartDate, 3, context);
            existingPlayer3.Depth.Should().Be(4);
            var existingPlayer1 = await GetExistingPlayer(positionCode, weekStartDate, 1, context);
            existingPlayer1.Depth.Should().Be(1);
        }

        private static async Task<ChartPositionDepth> GetExistingPlayer(string positionCode, DateTime weekStartDate, int playerId, DepthChartDbContext context)
        {
            var existingPlayer = await context.ChartPositionDepths.FirstOrDefaultAsync(
                x => x.SportCode == SportCode 
                && x.TeamCode == TeamCode 
                && x.WeekStartDate == weekStartDate 
                && x.PositionCode == positionCode 
                && x.PlayerId == playerId);
            await context.Entry(existingPlayer).ReloadAsync();
            return existingPlayer;
        }

        private static async Task<AddPlayerToDepthChartResponse?> GetResponseData(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<AddPlayerToDepthChartResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return responseData;
        }
    }
}