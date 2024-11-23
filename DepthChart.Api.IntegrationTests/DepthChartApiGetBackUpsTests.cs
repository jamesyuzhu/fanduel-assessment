using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DepthChart.Api.IntegrationTests
{
    [TestClass]
    public class DepthChartApiGetBackUpsTests
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private const string SportCode = "NFL";
        private const string TeamCode = "TampaBayBuccaneers";
        private const string RootUrl = $"/api/depthchart/backups/{SportCode}/{TeamCode}";
        private readonly Util _util;

        public DepthChartApiGetBackUpsTests()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            _util = new Util(SportCode, TeamCode);
        }                

        [TestMethod]
        public async Task GetBackUps_PositionCodeIsBlank_ShouldReturnStatusBadRequest()
        {             
            // Arrange
            var positionCode = string.Empty;
            var playerId = 1;             

            // Act
            var response = await _client.GetAsync($"{RootUrl}?positionCode={positionCode}&playerId={playerId}");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task GetBackUps_PlayerIdIsBlank_ShouldReturnStatusBadRequest()
        {
            // Arrange
            var positionCode = "GBUA";

            // Act
            var response = await _client.GetAsync($"{RootUrl}?positionCode={positionCode}&playerId=");

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task GetBackUps_PlayerInMiddle_ShouldReturnSuccessors()
        {
            // Arrange
            var positionCode = "GBUA";             

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, null, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, null, "Jessy Wu");
            await _util.CreateChartPositionDepthRecord(positionCode, 3, 3, context, null, "Mark Fang");
            
            // Act
            var response = await _client.GetAsync($"{RootUrl}?positionCode={positionCode}&playerId=1");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PlayerResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(2); 
            responseData[0].PlayerId.Should().Be(2);
            responseData[1].PlayerId.Should().Be(3);
        }

        [TestMethod]
        public async Task GetBackUps_PlayerWithoutSuccessor_ShouldReturnEmptyList()
        {
            // Arrange
            var positionCode = "GBUW";

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, null, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, null, "Jessy Wu");             

            // Act
            var response = await _client.GetAsync($"{RootUrl}?positionCode={positionCode}&playerId=2");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PlayerResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(0);            
        }

        [TestMethod]
        public async Task GetBackUps_PlayerNotExist_ShouldReturnEmptyList()
        {
            // Arrange
            var positionCode = "GBUN";

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, null, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, null, "Jessy Wu");

            // Act
            var response = await _client.GetAsync($"{RootUrl}?positionCode={positionCode}&playerId=4");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PlayerResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetBackUps_PlayerInMiddleAndChartDateIsGiven_ShouldReturnSuccessors()
        {
            // Arrange
            var positionCode = "GBUAC";
            var chartDate = DateTime.Today.AddDays(-14);

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode, 1, 1, context, chartDate, "Susan Lee");
            await _util.CreateChartPositionDepthRecord(positionCode, 2, 2, context, chartDate, "Jessy Wu");
            await _util.CreateChartPositionDepthRecord(positionCode, 3, 3, context, chartDate, "Mark Fang");

            // Act
            var response = await _client.GetAsync($"{RootUrl}?positionCode={positionCode}&playerId=1&chartDate={chartDate.ToString("yyyy-MM-dd")}");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PlayerResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(2);
            responseData[0].PlayerId.Should().Be(2);
            responseData[1].PlayerId.Should().Be(3);
        }
    }
}
