namespace Blaise.Api.Contracts.Interfaces
{
    public interface IConfigurationProvider
    {
        string BaseUrl { get; }
        string TempPath { get; }
        string DqsBucket { get; }
        string PackageExtension { get; }
    }
}