using System.IO;
using System.IO.Compression;
using Blaise.Api.Logging.Services;

namespace Blaise.Api.Tests.Behaviour.Helpers.Extensions
{
    public static class FileExtensions
    {
        public static string ExtractFiles(this string sourceFilePath, string destinationFilePath)
        {
            if (Directory.Exists(destinationFilePath))
            {
                Directory.Delete(destinationFilePath, true);
            }

            ZipFile.ExtractToDirectory(sourceFilePath, destinationFilePath);

            return destinationFilePath;
        }

        public static void ZipFiles(this string sourceFilePath, string destinationFilePath)
        {
            var _logging = new EventLogging();
            _logging.LogInfo($"Create zip '{destinationFilePath}' from '{sourceFilePath}'");
            ZipFile.CreateFromDirectory(sourceFilePath, destinationFilePath);
        }
    }
}
