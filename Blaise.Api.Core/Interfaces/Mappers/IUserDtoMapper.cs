using System.Collections.Generic;
using Blaise.Api.Contracts.Models.User;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Interfaces.Mappers
{
    public interface IUserDtoMapper
    {
        UserDto MapToUserDto(IUser user);
        IEnumerable<UserDto> MapToUserDtos(IEnumerable<IUser> users);
    }
}