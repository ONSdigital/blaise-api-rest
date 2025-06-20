using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blaise.Api.Storage.Interfaces
{
    public interface ICloudStorageClientProvider
    {
        Task<IEnumerable<string>> GetListOfFiles(string bucketName, string bucketPath);

        Task DownloadAsync(string bucketPath, string fileName, string destinationFilePath);
    }
}