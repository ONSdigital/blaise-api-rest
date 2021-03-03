using System;
using System.IO;
using System.Threading;
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

        public void CleanUpTempFiles(string path)
        {
            if (!Directory.Exists(path)) return;

            try
            {
                Thread.Sleep(10000);
                DeleteDirectoryAndFilesInPath(path);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not cleanup folders, {e.Message}, {e}");
            }
        }

        public static void DeleteDirectoryAndFilesInPath(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            foreach (var dir in dirInfo.GetDirectories())
            {
                foreach (var file in dir.GetFiles())
                {
                    file.Delete();
                }

                dir.Delete(true);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                File.Delete(file);
            }
        }
    }
}