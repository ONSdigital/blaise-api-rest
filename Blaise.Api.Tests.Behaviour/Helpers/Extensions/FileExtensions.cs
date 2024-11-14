using System.IO;
using System.IO.Compression;

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
            ZipFile.CreateFromDirectory(sourceFilePath, destinationFilePath);
        }
    }
}
