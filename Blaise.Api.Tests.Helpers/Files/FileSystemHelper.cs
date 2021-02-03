using System.IO;
using Blaise.Api.Tests.Helpers.Configuration;

namespace Blaise.Api.Tests.Helpers.Files
{
    public class FileSystemHelper
    {
        private static FileSystemHelper _currentInstance;

        public static FileSystemHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new FileSystemHelper());
        }

        public void CleanUpTempFiles()
        {
            if (Directory.Exists(BlaiseConfigurationHelper.TempDownloadPath))
            {
                foreach (var directory in Directory.GetDirectories(BlaiseConfigurationHelper.TempDownloadPath))
                {
                    Directory.Delete(directory, true);
                }

                foreach (var file in Directory.GetFiles(BlaiseConfigurationHelper.TempDownloadPath))
                {
                    File.Delete(file);
                }
            }
        }
    }
}
