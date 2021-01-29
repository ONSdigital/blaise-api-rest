using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaise.Api.Tests.Helpers.Extensions
{
       public static class ArgumentValidationExtensions
    {
        public static void ThrowExceptionIfNullOrEmpty(this string argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (string.IsNullOrWhiteSpace(argument))
            {

                throw new ArgumentException($"A value for the argument '{argumentName}' must be supplied");
            }
        }

        public static void ThrowExceptionIfNullOrEmpty(this IEnumerable<string> argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (!argument.Any())
            {

                throw new ArgumentException($"A value for the argument '{argumentName}' must be supplied");
            }
        }

        public static void ThrowExceptionIfNull<T>(this T argument, string argumentName)
        {
            if (argument == null)
            {
                throw new ArgumentNullException($"The argument '{argumentName}' must be supplied");
            }
        }
    }
}