using Moq;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using UserApi.Entities;
using UserApi.Models;
using UserApi.Services;
using UserApi.Repositories;

using TbspRpgLib.Settings;

using Xunit;

namespace UserApi.Tests.Services {
    public class UserServiceTests : InMemoryTest {
        #region Setup
        private Guid _userOneId;
        public UserServiceTests() : base("UserServiceTests")
        {
            Seed();
        }

        private void Seed()
        {
            using var context = new UserContext(_dbContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            _userOneId = Guid.NewGuid();
            var user = new User
            {
                Id = _userOneId,
                Username = "test",
                Password = "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4="
            };

            var userTwo = new User
            {
                Id = Guid.NewGuid(),
                Username = "testTwo",
                Password = "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4="
            };
            
            context.AddRange(user, userTwo);
            context.SaveChanges();
        }
        #endregion
        
        private UserService CreateUserService(UserContext context)
        {
            var settings = new DatabaseSettings()
            {
                Salt = "y728sfLla98YUZpTgCM4VA=="
            };
            var repository = new UserRepository(context);
            return new UserService(settings, repository);
        }
        

        [Fact]
        public async void Authenticate_IsValid_ReturnResponse() {
            //arrange
            var req = new AuthenticateRequest() {
                Username = "test",
                Password = "test"
            };
            await using var context = new UserContext(_dbContextOptions);
            var userService = CreateUserService(context);

            //act
            var response = await userService.Authenticate(req);

            //assert
            Assert.NotNull(response);
            Assert.Equal("test", response.Username);
            Assert.Equal(_userOneId.ToString(), response.Id);
        }

        [Fact]
        public async void Authenticate_InvalidPassword_ReturnNull() {
            //arrange
            AuthenticateRequest req = new AuthenticateRequest() {
                Username = "test",
                Password = "testt"
            };
            await using var context = new UserContext(_dbContextOptions);
            var userService = CreateUserService(context);

            //act
            var response = await userService.Authenticate(req);

            //assert
            Assert.Null(response);
        }

        [Fact]
        public async void Authenticate_InvalidUsername_ReturnNull() {
            //arrange
            AuthenticateRequest req = new AuthenticateRequest() {
                Username = "testt",
                Password = "test"
            };
            await using var context = new UserContext(_dbContextOptions);
            var userService = CreateUserService(context);

            //act
            var response = await userService.Authenticate(req);

            //assert
            Assert.Null(response);
        }

        [Fact]
        public async void GetById_ValidId_ReturnUser() {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var userService = CreateUserService(context);

            //act
            var user = await userService.GetById(_userOneId.ToString());

            //assert
            Assert.Equal(_userOneId.ToString(), user.Id);
            Assert.Equal("test", user.Username);
        }

        [Fact]
        public async void GetById_InvalidId_ReturnNull() {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var userService = CreateUserService(context);

            //act
            var user = await userService.GetById(Guid.NewGuid().ToString());

            //assert
            Assert.Null(user);
        }

        [Fact]
        public async void GetAll_ReturnAll() {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var userService = CreateUserService(context);
            
            //act
            var users = await userService.GetAll();

            //assert
            Assert.Equal(2, users.Count);
        }
    }
}