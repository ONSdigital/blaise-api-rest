﻿using System.Collections.Generic;
using Blaise.Api.Contracts.Models.User;
using Blaise.Api.Core.Extensions;
using Blaise.Api.Core.Interfaces.Mappers;
using Blaise.Api.Core.Interfaces.Services;
using Blaise.Nuget.Api.Contracts.Interfaces;

namespace Blaise.Api.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IBlaiseUserApi _blaiseApi;
        private readonly IUserDtoMapper _mapper;

        public UserService(
            IBlaiseUserApi blaiseApi, 
            IUserDtoMapper mapper)
        {
            _blaiseApi = blaiseApi;
            _mapper = mapper;
        }

        public IEnumerable<UserDto> GetUsers()
        {
            var user = _blaiseApi.GetUsers();

            return _mapper.MapToUserDtos(user);
        }

        public UserDto GetUser(string userName)
        {
            userName.ThrowExceptionIfNullOrEmpty("userName");
            var user = _blaiseApi.GetUser(userName);

            return _mapper.MapToUserDto(user);
        }

        public bool UserExists(string userName)
        {
            userName.ThrowExceptionIfNullOrEmpty("userName");

            return _blaiseApi.UserExists(userName);
        }

        public void AddUser(AddUserDto addUserDto)
        {
            addUserDto.Name.ThrowExceptionIfNullOrEmpty("addUserDto.Name");
            addUserDto.Password.ThrowExceptionIfNullOrEmpty("addUserDto.Password");
            addUserDto.Role.ThrowExceptionIfNullOrEmpty("addUserDto.Role");
            addUserDto.ServerParks.ThrowExceptionIfNullOrEmpty("addUserDto.ServerParks");

            _blaiseApi.AddUser(addUserDto.Name, addUserDto.Password, addUserDto.Role, addUserDto.ServerParks, addUserDto.DefaultServerPark);
        }

        public void UpdatePassword(string userName, UpdateUserPasswordDto updateUserPasswordDto)
        {
            userName.ThrowExceptionIfNullOrEmpty("userName");
            updateUserPasswordDto.Password.ThrowExceptionIfNullOrEmpty("updateUserPasswordDto.Password");

            _blaiseApi.UpdatePassword(userName, updateUserPasswordDto.Password);
        }

        public void UpdateRole(string userName, UpdateUserRoleDto updateUserRoleDto)
        {
            userName.ThrowExceptionIfNullOrEmpty("userName");
            updateUserRoleDto.Role.ThrowExceptionIfNullOrEmpty("updateUserRoleDto.Role");

            _blaiseApi.UpdateRole(userName, updateUserRoleDto.Role);
        }

        public void UpdateServerParks(string userName, UpdateUserServerParksDto updateUserServerParksDto)
        {
            userName.ThrowExceptionIfNullOrEmpty("userName");
            updateUserServerParksDto.ServerParks.ThrowExceptionIfNullOrEmpty("updateUserServerParksDto.ServerParks");

            _blaiseApi.UpdateServerParks(userName, updateUserServerParksDto.ServerParks, updateUserServerParksDto.DefaultServerPark);
        }
        public void RemoveUser(string userName)
        {
            userName.ThrowExceptionIfNullOrEmpty("userName");

            _blaiseApi.RemoveUser(userName);
        }
    }
}
