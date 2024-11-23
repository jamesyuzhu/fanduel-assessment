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

        [TestMethod]
        public async Task GetFullDepthChart_WithData_ShouldReturnFullList()
        {
            // Arrange
            var positionCode1 = "GF1";
            var positionCode2 = "GF2";
            var chartDate = DateTime.Today.AddDays(-7);

            // Seed data           
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            var allRecords = context.ChartPositionDepths.ToList();
            context.ChartPositionDepths.RemoveRange(allRecords);
            context.SaveChanges();

            await _util.CreateChartPositionDepthRecord(positionCode1, 1, 1, context, chartDate, "Tester1");
            await _util.CreateChartPositionDepthRecord(positionCode1, 2, 2, context, chartDate, "Tester2");
            await _util.CreateChartPositionDepthRecord(positionCode1, 3, 3, context, chartDate, "Tester3");

            await _util.CreateChartPositionDepthRecord(positionCode2, 4, 1, context, chartDate, "Tester4");
            await _util.CreateChartPositionDepthRecord(positionCode2, 5, 2, context, chartDate, "Tester5");
            await _util.CreateChartPositionDepthRecord(positionCode2, 6, 3, context, chartDate, "Tester6");

            // Act            
            var response = await _client.GetAsync($"{RootUrl}?chartDate={chartDate.ToString("yyyy-MM-dd")}");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PositionDepthResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(6);
            
            responseData.Where(x => x.PositionCode == positionCode1).ToList().Count.Should().Be(3);
            responseData.Where(x => x.PositionCode == positionCode2).ToList().Count.Should().Be(3);             
        }

        [TestMethod]
        public async Task GetFullDepthChart_WithoutData_ShouldReturnEmptyList()
        {
            // Arrange
            var positionCode1 = "GF3";
            var positionCode2 = "GF4";
            var chartDate = DateTime.Today.AddDays(-14);

            // Seed data           
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DepthChartDbContext>();
            var allRecords = context.ChartPositionDepths.ToList();
            context.ChartPositionDepths.RemoveRange(allRecords);
            context.SaveChanges();

            await _util.CreateChartPositionDepthRecord(positionCode1, 1, 1, context, null, "Tester1");
            await _util.CreateChartPositionDepthRecord(positionCode1, 2, 2, context, null, "Tester2");
            await _util.CreateChartPositionDepthRecord(positionCode1, 3, 3, context, null, "Tester3");

            await _util.CreateChartPositionDepthRecord(positionCode2, 4, 1, context, null, "Tester4");
            await _util.CreateChartPositionDepthRecord(positionCode2, 5, 2, context, null, "Tester5");
            await _util.CreateChartPositionDepthRecord(positionCode2, 6, 3, context, null, "Tester6");

            // Act            
            var response = await _client.GetAsync($"{RootUrl}?chartDate={chartDate.ToString("yyyy-MM-dd")}");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseData = await _util.GetResponseData<List<PositionDepthResponse>>(response);
            responseData.Should().NotBeNull();
            responseData.Count.Should().Be(0);
        }
    }
}
