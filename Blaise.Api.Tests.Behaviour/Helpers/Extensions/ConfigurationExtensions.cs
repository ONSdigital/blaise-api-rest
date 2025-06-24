using System;
using System.Configuration;

namespace Blaise.Api.Tests.Behaviour.Helpers.Extensions
{
    public class ConfigurationExtensions
    {
        public static string GetEnvironmentVariable(string variableName)
        {
            var variable = Environment.GetEnvironmentVariable(variableName) ?? GetVariable(variableName);
            variable.ThrowExceptionIfNullOrEmpty(variableName);
            return variable;
        }

        public static string GetVariable(string variableName)
        {
            var variable = ConfigurationManager.AppSettings[variableName];
            variable.ThrowExceptionIfNullOrEmpty(variableName);
            return variable;
        }
    }
}
