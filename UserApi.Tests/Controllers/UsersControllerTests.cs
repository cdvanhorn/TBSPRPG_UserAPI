using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TbspRpgLib.Settings;
using UserApi.Controllers;
using UserApi.Entities;
using UserApi.Models;
using UserApi.Repositories;
using UserApi.Services;
using UserApi.ViewModels;
using Xunit;

namespace UserApi.Tests.Controllers
{
    public class UsersControllerTests : InMemoryTest
    {
        #region Setup
        private Guid _userOneId;
        public UsersControllerTests() : base("UsersControllerTests")
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
        
        private UsersController CreateController(UserContext context)
        {
            var settings = new DatabaseSettings()
            {
                Salt = "y728sfLla98YUZpTgCM4VA=="
            };
            var repository = new UserRepository(context);
            var service = new UserService(settings, repository);
            return new UsersController(service);
        }
        
        #region Authenticate
        [Fact]
        public async Task Authenticate_Valid_ReturnResponse()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var controller = CreateController(context);
            var authRequest = new AuthenticateRequest()
            {
                Username = "test",
                Password = "test"
            };
            
            //act
            var response = await controller.Authenticate(authRequest);
            
            //assert
            var okObjectResult = response as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var authResponse = okObjectResult.Value as AuthenticateResponse;
            Assert.NotNull(authResponse);
            Assert.Equal("test", authResponse.Username);
            Assert.Equal(_userOneId.ToString(), authResponse.Id);
        }
        
        [Fact]
        public async Task Authenticate_InValid_ReturnBadResponse()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var controller = CreateController(context);
            var authRequest = new AuthenticateRequest()
            {
                Username = "test",
                Password = "testt"
            };
            
            //act
            var response = await controller.Authenticate(authRequest);
            
            //assert
            var badRequestResult = response as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }
        #endregion

        #region GetAll

        [Fact]
        public async Task GetAll_ReturnsAll()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var controller = CreateController(context);
            
            //act
            var users = await controller.GetAll();
            
            //assert
            //assert
            var okObjectResult = users as OkObjectResult;
            Assert.NotNull(okObjectResult);
            var response = okObjectResult.Value as IEnumerable<UserViewModel>;
            Assert.NotNull(response);
            Assert.Equal(2, response.Count());
            Assert.Equal("test", response.First().Username);
            Assert.Equal(_userOneId.ToString(), response.First().Id);
        }
        #endregion

        // [Fact]
        // public async Task GetUserById_InValid_ReturnNone()
        // {
        //     //arrange
        //     await using var context = new UserContext(_dbContextOptions);
        //     var repo = new UserRepository(context);
        //     
        //     //act
        //
        //     //assert
        // }
    }
}