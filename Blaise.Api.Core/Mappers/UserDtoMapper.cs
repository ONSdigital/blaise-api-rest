using System.Collections.Generic;
using System.Linq;
using Blaise.Api.Contracts.Models.User;
using Blaise.Api.Core.Interfaces.Mappers;
using StatNeth.Blaise.API.ServerManager;

namespace Blaise.Api.Core.Mappers
{
    public class UserDtoMapper : IUserDtoMapper
    {
        public IEnumerable<UserDto> MapToUserDtos(IEnumerable<IUser> users)
        {
            var userList = new List<UserDto>();

            foreach (var user in users)
            {
                userList.Add(MapToUserDto(user));
            }

            return userList;
        }

        public UserDto MapToUserDto(IUser user)
        {
            return new UserDto
            {
                Name = user.Name,
                Role = GetRole(user as IUser2),
                ServerParks = user.ServerParks.ToList()
            };
        }

        private static string GetRole(IUser2 user)
        {
            return user.Role;
        }
    }
}
