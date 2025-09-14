namespace Blaise.Api.Contracts.Interfaces
{
    public interface IConfigurationProvider
    {
        string BaseUrl { get; }

        string TempPath { get; }

        string PackageExtension { get; }

        string DqsBucket { get; }

        string NisraBucket { get; }

        string IngestBucket { get; }
    }
}
