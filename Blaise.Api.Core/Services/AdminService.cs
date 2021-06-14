using System.Collections.Generic;
using Blaise.Api.Contracts.Models.Admin;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IBlaiseAdminApi _adminApi;
        private readonly IOpenConnectionModelMapper _mapper;

        public AdminService(
            IBlaiseAdminApi adminApi, 
            IOpenConnectionModelMapper mapper)
        {
            _adminApi = adminApi;
            _mapper = mapper;
        }

        public IEnumerable<OpenConnectionDto> GetOpenConnections()
        {
            var openConnectionModels = _adminApi.OpenConnections();

            return _mapper.MapTOpenConnectionDtos(openConnectionModels);
        }

        public void ResetConnections()
        {
            _adminApi.ResetConnections();
        }
    }
}
