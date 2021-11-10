using System;
using System.IO;
using System.Threading;

namespace Blaise.Api.Extensions
{
    public static class FileSystemExtensions
    {
        public static void CleanUpTempFiles(this string path)
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

        private static void CleanUpFiles(string path)
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