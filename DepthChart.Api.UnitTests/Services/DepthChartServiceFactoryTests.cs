using System;
using System.Collections.Generic;
using DepthChart.Api.Services;
using DepthChart.Api.Services.Interface;
using DepthChart.Api.Repositories;
using Moq;
using Xunit;

namespace DepthChart.Api.UnitTests.Services
{   
    public class DepthChartServiceFactoryTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IDepthChartRepository> _mockRepository;
        private readonly DepthChartServiceFactory _factory;

        public DepthChartServiceFactoryTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockRepository = new Mock<IDepthChartRepository>();

            // Mock the service provider to return the repository
            _mockServiceProvider
                .Setup(sp => sp.GetService(typeof(IDepthChartRepository)))
                .Returns(_mockRepository.Object);

            // Initialize the factory
            _factory = new DepthChartServiceFactory(_mockServiceProvider.Object);
        }

        [Fact]
        public void Create_ThrowsArgumentNullException_WhenSportCodeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _factory.Create(null, "Bulls"));
        }

        [Fact]
        public void Create_ThrowsArgumentNullException_WhenTeamCodeIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _factory.Create("NFL", null));
        }

        [Fact]
        public void Create_ThrowsInvalidOperationException_WhenNoServiceIsFound()
        {          
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _factory.Create("NFL", "Bulls"));
        }

        [Fact]
        public void Create_ReturnsCorrectServiceInstance()
        {
            // Arrange
            var mockService = new Mock<IDepthChartService>();
            mockService.Setup(s => s.SportCode).Returns("NFL");
            mockService.Setup(s => s.TeamCodes).Returns(new List<string> { "TeamA" });

            // Mock the service provider to return the service instance
            _mockServiceProvider
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(mockService.Object);

            // Manually load the service type into the private static registry
            var serviceType = mockService.Object.GetType();
            var registry = typeof(DepthChartServiceFactory)
                .GetField("_serviceRegistry", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .GetValue(null) as Dictionary<string, Type>;

            registry.Add("nfl-teama", serviceType);

            // Act
            var service = _factory.Create("NFL", "TeamA");

            // Assert
            Assert.NotNull(service);             
            Assert.Equal("NFL", service.SportCode);
            Assert.Equal("TeamA", service.TeamCodes[0]);
        }
    }
}


