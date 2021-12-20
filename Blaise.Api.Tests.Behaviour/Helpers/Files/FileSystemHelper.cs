using Blaise.Api.Extensions;

namespace Blaise.Api.Tests.Behaviour.Helpers.Files
{
    public class FileSystemHelper
    {
        private static FileSystemHelper _currentInstance;

        public static FileSystemHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new FileSystemHelper());
        }

        public void CleanUpTempFiles(string path)
        {
            path.CleanUpTempFiles();
        }
    }
}