using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DepthChart.Api.IntegrationTests
{
    [TestClass]
    public class DepthChartApiRemovePlayerTests
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private const string SportCode = "NFL";
        private const string TeamCode = "TampaBayBuccaneers";
        private const string RootUrl = $"/api/depthchart/{SportCode}/{TeamCode}";
        private readonly Util _util;

        public DepthChartApiRemovePlayerTests()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            _util = new Util(SportCode, TeamCode);
        }                

        [TestMethod]
        public async Task RemovePlayerFromDepthChart_PositionCodeIsBlank_ShouldReturnStatusBadRequest()
        {             
            // Arrange
            var positionCode = string.Empty;
            var playerId = 1;             

            // Act
            var response = await _client.DeleteAsync($"{RootUrl}?positionCode={positionCode}&playerId={playerId}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task RemovePlayerFromDepthChart_PlayerIdIsBlank_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var positionCode = "LT";            

            // Act
            var response = await _client.DeleteAsync($"{RootUrl}?positionCode={positionCode}&playerId=");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task RemovePlayerFromDepthChart_PlayerInMiddle_ShouldRemovePlayerAndShifeSuccessors()
        {
            // Arrange
            var positionCode = "RMD";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, null, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, null, "Jessy Wu");
            await _util.CreateChartPositionDepthRecord(positionCode, 3, 3, context, null, "Mark Fang");            

            // Act
            var response = await _client.DeleteAsync($"{RootUrl}?positionCode={positionCode}&playerId=2");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<PlayerResponse>(response);
            responseData.Should().NotBeNull();
            responseData?.PlayerId.Should().Be(2);            
            var existingPlayer1 = await _util.GetExistingPlayer(positionCode, weekStartDate, 1, context);
            existingPlayer1?.Depth.Should().Be(1);
            var existingPlayer3 = await _util.GetExistingPlayer(positionCode, weekStartDate, 3, context);
            existingPlayer3?.Depth.Should().Be(2);             
        }

        [TestMethod]
        public async Task RemovePlayerFromDepthChart_PlayerNotInPosition_ShouldReturnEmptyResponse()
        {
            // Arrange
            var positionCode = "NP";
            var weekStartDate = DepthChart.Api.Services.NFLDepthChartService.GetWeekStartDate();

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, null, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, null, "Jessy Wu");
            await _util.CreateChartPositionDepthRecord(positionCode, 3, 3, context, null, "Mark Fang");

            // Act
            var response = await _client.DeleteAsync($"{RootUrl}?positionCode={positionCode}&playerId=4");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<PlayerResponse>(response);
            responseData.Should().NotBeNull();
            responseData?.PlayerId.Should().BeNull();
            responseData?.PlayerName.Should().BeNull();
            var existingPlayer1 = await _util.GetExistingPlayer(positionCode, weekStartDate, 1, context);
            existingPlayer1?.Depth.Should().Be(1);
            var existingPlayer2 = await _util.GetExistingPlayer(positionCode, weekStartDate, 2, context);
            existingPlayer2?.Depth.Should().Be(2);
            var existingPlayer3 = await _util.GetExistingPlayer(positionCode, weekStartDate, 3, context);
            existingPlayer3?.Depth.Should().Be(3);
        }

        //private static async Task<ChartPositionDepth> GetExistingPlayer(string positionCode, DateTime weekStartDate, int playerId, DepthChartDbContext context)
        //{
        //    var existingPlayer = await context.ChartPositionDepths.FirstOrDefaultAsync(
        //        x => x.SportCode == SportCode
        //        && x.TeamCode == TeamCode
        //        && x.WeekStartDate == weekStartDate
        //        && x.PositionCode == positionCode
        //        && x.PlayerId == playerId);
        //    await context.Entry(existingPlayer).ReloadAsync();
        //    return existingPlayer;
        //}

        //private static async Task<PlayerResponse> GetResponseData(HttpResponseMessage response)
        //{
        //    var responseBody = await response.Content.ReadAsStringAsync();
        //    var responseData = JsonSerializer.Deserialize<PlayerResponse>(responseBody, new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    });
        //    return responseData;
        //}
    }
}
