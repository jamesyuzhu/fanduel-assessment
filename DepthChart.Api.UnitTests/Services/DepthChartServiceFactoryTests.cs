using DepthChart.Api.Dtos.Requests;
using DepthChart.Api.Dtos.Responses;
using DepthChart.Api.Repositories;
using DepthChart.Api.Services;
using DepthChart.Api.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DepthChart.Api.UnitTests.Services
{
    public class DepthChartServiceFactoryTests
    {
        private readonly IServiceProvider _serviceProvider;

        public DepthChartServiceFactoryTests()
        {
            // Set up a service collection
            var services = new ServiceCollection();

            // Add In-Memory DbContext
            services.AddDbContext<DepthChartDbContext>(options =>
                options.UseInMemoryDatabase("TestDatabase"));

            // Register concrete implementations of IDepthChartService
            services.AddTransient<Test1DepthChartService>();
            services.AddTransient<Test2DepthChartService>();

            // Build the service provider
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void Create_ServiceRegisteredForSportAndTeam_ReturnsExpectedService()
        {
            // Arrange
            var factory = new DepthChartServiceFactory(_serviceProvider);

            // Act
            var result = factory.Create("NFL", "Patriots");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("NFL", result.SportCode);
        }

        [Fact]
        public void Create_SportCodeMissed_ThrowsArgumentNullException()
        {
            // Arrange
            var factory = new DepthChartServiceFactory(_serviceProvider);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.Create(null, "Bulls"));
        }

        [Fact]
        public void Create_TeamCodeMissed_ThrowsArgumentNullException()
        {
            // Arrange
            var factory = new DepthChartServiceFactory(_serviceProvider);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => factory.Create("NFL", null));
        }

        [Fact]
        public void Create_ServiceNotRegistered_ThrowsInvalidOperationException()
        {
            // Arrange
            var factory = new DepthChartServiceFactory(_serviceProvider);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => factory.Create("NBA", "Bulls"));
        }


        [Fact]
        public void Create_LoadServiceTypes_CorrectlyPopulatesRegistry()
        {
            // Arrange             
            var factory = new DepthChartServiceFactory(_serviceProvider);

            // Act
            var nflService = factory.Create("NFL", "Patriots");
            var nbaService = factory.Create("NBA", "Lakers");

            // Assert
            Assert.NotNull(nflService);
            Assert.NotNull(nbaService);
            Assert.Equal("NFL", nflService.SportCode);
            Assert.Equal("NBA", nbaService.SportCode);
        }        
    }

    public class Test1DepthChartService : IDepthChartService
    {
        public string SportCode => "NFL";
        public List<string> TeamCodes => new() { "Patriots" };

        public Test1DepthChartService(DepthChartDbContext dbContext)
        {
            // Optionally, use the DbContext in the implementation
        }

        public Task<AddPlayerToDepthChartResponse> AddPlayerToDepthChartAsync(AddPlayerToDepthChartRequest request, string teamCode)
        {
            throw new NotImplementedException();
        }

        public Task<PlayerResponse> RemovePlayerFromDepthChartAsync(RemovePlayerFromDepthChartRequest request, string teamCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerResponse>> GetBackupsAsync(GetBackUpsRequest request, string teamCode)
        {
            throw new NotImplementedException();
        }
    }

    public class Test2DepthChartService : IDepthChartService
    {
        public string SportCode => "NBA";
        public List<string> TeamCodes => new() { "Lakers" };

        public Test2DepthChartService(DepthChartDbContext dbContext)
        {
            // Optionally, use the DbContext in the implementation
        }

        public Task<AddPlayerToDepthChartResponse> AddPlayerToDepthChartAsync(AddPlayerToDepthChartRequest request, string teamCode)
        {
            throw new NotImplementedException();
        }

        public Task<PlayerResponse> RemovePlayerFromDepthChartAsync(RemovePlayerFromDepthChartRequest request, string teamCode)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlayerResponse>> GetBackupsAsync(GetBackUpsRequest request, string teamCode)
        {
            throw new NotImplementedException();
        }
    }

}
