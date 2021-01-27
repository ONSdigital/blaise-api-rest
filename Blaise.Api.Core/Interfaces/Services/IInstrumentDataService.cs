using System.Threading.Tasks;
using Blaise.Api.Contracts.Models.Instrument;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IInstrumentDataService
    {
        Task<string> DeliverInstrumentPackageWithDataAsync(string serverParkName, InstrumentPackageDto instrumentPackageDto);

        Task<string> DownloadInstrumentPackageWithDataAsync(string serverParkName, InstrumentPackageDto instrumentPackageDto);
    }
}