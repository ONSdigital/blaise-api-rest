
using System;
using System.IO;
using System.Threading;

namespace Blaise.Api.Tests.Behaviour.Helpers.Files
{
    public class FileSystemHelper
    {
        private static FileSystemHelper _currentInstance;

        public static FileSystemHelper GetInstance()
        {
            return _currentInstance ?? (_currentInstance = new FileSystemHelper());
        }

        public void CleanUpTempFiles( string path)
        {
            if (!Directory.Exists(path)) return;

            var directoryInfo = new DirectoryInfo(path);

            if (directoryInfo.Parent != null &&
                Guid.TryParse(Path.GetDirectoryName(directoryInfo.Parent.Name), out _))
            {
                CleanUpFiles(directoryInfo.Parent.FullName);
                return;
            }

            CleanUpFiles(path);
        }

        private void CleanUpFiles(string path)
        {
            try
            {
                Thread.Sleep(2000);
                Directory.Delete(path, true);
            }
            catch //ewwwwwww fml
            {
            }
        }
    }
}