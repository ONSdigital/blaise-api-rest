using System;
using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Core.Interfaces.Services;
using StringComparison = System.StringComparison;

namespace Blaise.Api.Core.Services
{
    public class JakeService : IJakeService
    {

        private readonly List<string> _bestFriends;
        private readonly List<string> _naughtyList;

        public JakeService()
        {
            _bestFriends = new List<string> { "Jamie", "Nik", "Elinor", "Matthew", "Ali" };
            _naughtyList = new List<string> { "Richmond" };
        }

        public string GreetCustomer(string customerGreeting, string name)
        {
            if (NoGreetingOrNameSupplied(customerGreeting, name))
            {
                return "How may I help you?";
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                return "Good morning";
            }

            if (CustomerIsOnBestFriendsList(name))
            {
                return $"Hiya {name}, great to see you!";
            }

            if (CustomerIsOnNaughtyList(name))
            {
                return "And what do you think you are doing here?!";
            }

            if (string.IsNullOrWhiteSpace(customerGreeting))
            {
                return $"How may I help you {name}?";
            }

            if (CustomerSaysHello(customerGreeting))
            {
                return string.IsNullOrWhiteSpace(name) ? "Hi there" : $"Hi {name}";
            }

            return $"Hey {name}";
        }

        private bool CustomerIsOnBestFriendsList(string name)
        {
            return _bestFriends.Any(b => b.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        private bool CustomerIsOnNaughtyList(string name)
        {
            return _naughtyList.Any(n => n.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool CustomerSaysHello(string customerGreeting)
        {
            return customerGreeting.Equals("hello", StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool NoGreetingOrNameSupplied(string customerGreeting, string name)
        {
            return string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(customerGreeting);
        }
    }
}
