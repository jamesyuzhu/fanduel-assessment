using DepthChart.Api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace DepthChart.Api.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {        

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registrations
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DepthChartDbContext));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory DbContext for testing
                services.AddDbContext<DepthChartDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");                    
                });
            });
        }
    }
}
