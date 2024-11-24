using DepthChart.Api.Services.Interface;
using System.Collections.Generic;
using System;
using System.Linq;
using DepthChart.Api.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DepthChart.Api.Services
{
    public class DepthChartServiceFactory
    {        
        private static readonly Dictionary<string, Type> _serviceRegistry = new();
        private static bool _initialized = false;

        private readonly IServiceProvider _serviceProvider;

        public DepthChartServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            if (!_initialized)
            {
                lock (_serviceRegistry)
                {
                    if (!_initialized)
                    {
                        LoadServiceTypes();
                        _initialized = true;
                    }
                }
            }
        }

        private void LoadServiceTypes()
        {
            var serviceType = typeof(IDepthChartService);

            // Find all types implementing IDepthChartService
            var implementationTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => serviceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            
            foreach (var type in implementationTypes)
            {
                // Instantiate the type to read Sport and TeamCode properties
                var instance = (IDepthChartService)Activator.CreateInstance(type, _serviceProvider.GetService(typeof(IDepthChartRepository)));
                // Same sport service can support multiple teams. Map the sportCode and teamCode combination to the same type 
                foreach(var teamCode in instance.TeamCodes)
                {
                    var key = $"{instance.SportCode.ToLowerInvariant()}-{teamCode.ToLowerInvariant()}";
                    _serviceRegistry[key] = type;
                }                
            }
        }

        public IDepthChartService Create(string sportCode, string teamCode)
        {
            if (string.IsNullOrEmpty(sportCode))
            {
                throw new ArgumentNullException(nameof(sportCode));
            }
            if (string.IsNullOrEmpty(teamCode))
            {
                throw new ArgumentNullException(nameof(teamCode));
            }

            var key = $"{sportCode.ToLowerInvariant()}-{teamCode.ToLowerInvariant()}";
            if (!_serviceRegistry.TryGetValue(key, out var type))
            {
                throw new InvalidOperationException($"No service found for Sport: {sportCode}, Team: {teamCode}");
            }

            // Resolve the service from the DI container
            return (IDepthChartService)_serviceProvider.GetRequiredService(type);
        }
    }
}
