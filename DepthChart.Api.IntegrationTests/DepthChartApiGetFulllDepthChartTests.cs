using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Repositories;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace DepthChart.Api.IntegrationTests
{
    [TestClass]
    public class DepthChartApiGetFullDepthChartTests
    {
        private readonly CustomWebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;
        private const string SportCode = "NFL";
        private const string TeamCode = "TampaBayBuccaneers";
        private const string RootUrl = $"/api/depthchart/full/{SportCode}/{TeamCode}";
        private readonly Util _util;

        public DepthChartApiGetFullDepthChartTests()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
            _client = _factory.CreateClient();
            _util = new Util(SportCode, TeamCode);
        }

        //[TestMethod]
        //public async Task GetFullDepthChart_TeamCodeIsNull_ShouldReturnStatusBadRequest()
        //{
             
        //    // Act
        //    var response = await _client.DeleteAsync($"/api/depthchart/full/{SportCode}/");

        //    // Assert
        //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        //}

        [TestMethod]
        public async Task GetFullDepthChart_WithData_ShouldReturnFullList()
        {
            // Arrange
            var positionCode1 = "GF1";
            var positionCode2 = "GF2";

            // Seed data
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            await _util.CreateChartPositionDepthRecord(positionCode1, 1, 1, context, "Tester1");
            await _util.CreateChartPositionDepthRecord(positionCode1, 2, 2, context, "Tester2");
            await _util.CreateChartPositionDepthRecord(positionCode1, 3, 3, context, "Tester3");

            await _util.CreateChartPositionDepthRecord(positionCode2, 4, 1, context, "Tester4");
            await _util.CreateChartPositionDepthRecord(positionCode2, 5, 2, context, "Tester5");
            await _util.CreateChartPositionDepthRecord(positionCode2, 6, 3, context, "Tester6");

            // Act
            var response = await _client.GetAsync($"{RootUrl}");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PositionDepthResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(6);
            
            responseData[0].PositionCode.Should().Be(positionCode1);
            responseData[0].PlayerId.Should().Be(1);
            responseData[0].PlayerName.Should().Be("Tester1");
            responseData[0].Depth.Should().Be(1);

            responseData[5].PositionCode.Should().Be(positionCode2);
            responseData[5].PlayerId.Should().Be(6);
            responseData[5].PlayerName.Should().Be("Tester6");
            responseData[5].Depth.Should().Be(3);
        }        
    }
}
