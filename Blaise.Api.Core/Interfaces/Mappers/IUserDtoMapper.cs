namespace Blaise.Api.Core.Interfaces.Mappers
{
    using System.Collections.Generic;
    using Blaise.Api.Contracts.Models.User;
    using StatNeth.Blaise.API.ServerManager;

    public interface IUserDtoMapper
    {
        UserDto MapToUserDto(IUser user);

        IEnumerable<UserDto> MapToUserDtos(IEnumerable<IUser> users);
    }
}
