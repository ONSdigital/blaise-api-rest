namespace Blaise.Api.Contracts.Interfaces
{
    public interface IConfigurationProvider
    {
        string BaseUrl { get; }
        string TempPath { get; }
        string BucketPath { get; }
        string PackageExtension { get; }
    }
}