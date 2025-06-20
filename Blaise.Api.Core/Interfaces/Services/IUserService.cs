using System.Collections.Generic;
using Blaise.Api.Contracts.Models.User;

namespace Blaise.Api.Core.Interfaces.Services
{
    public interface IUserService
    { 
        IEnumerable<UserDto> GetUsers();
        UserDto GetUser(string userName);
        bool UserExists(string userName);
        void AddUser(AddUserDto userDto);
        void RemoveUser(string userName);
        void UpdatePassword(string userName, string password);
        void UpdateRole(string userName, UpdateUserRoleDto roleDto);
        void UpdateServerParks(string userName, UpdateUserServerParksDto serverParksDto);

        bool ValidateUser(string userName, string password);
    }
}