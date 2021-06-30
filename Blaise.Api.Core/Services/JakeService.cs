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

            //var response = $"hey {name}";
            //var bResponse = $"Yooo its {name}";

            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogError("error", new ArgumentException());
                return null;
            }

            /*string[] bFriends = {"Aidan", "Caleb", "Chloe"};
            foreach (var i in bFriends)
            {
                for
            }
            */

            _logger.LogInfo($"Saying hello to {name}");

            return CheckName(name);
        }

        public string CheckName(string name)
        {
            var response = $"hey {name}";
            var bResponse = $"Yooo its {name}";

            //if (name == "aidan")
            //{
                
            //}

            switch (name.ToLower())
            {
                case "aidan":
                response = bResponse;
                break;

                case "chloe":
                response = $"Loves you {name}";
                break;

                case "caleb":
                    name = "bb";
                    response = $"Yooo its {name}";
                    break;
            }


            return response;
        }
    }
}
