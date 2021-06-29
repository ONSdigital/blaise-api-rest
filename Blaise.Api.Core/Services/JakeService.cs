using System;
using Blaise.Api.Contracts.Interfaces;
using Blaise.Api.Core.Interfaces.Services;

namespace Blaise.Api.Core.Services
{
    public class JakeService : IJakeService
    {
        private readonly ILoggingService _logger;

        public JakeService(ILoggingService logger)
        {
            _logger = logger;
        }

        public string HelloJake(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogError("error", new ArgumentException());
                return null;
            }

            
            _logger.LogInfo($"Saying hello to {name}");

            return $"hey {name}";
        }
    }
}
