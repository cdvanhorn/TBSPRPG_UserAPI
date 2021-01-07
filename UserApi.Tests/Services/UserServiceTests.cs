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
    public class UserServiceTests {
        private readonly UserService _userService;

        public UserServiceTests() {
            //need to mock database settings
            var mdb = new Mock<IDatabaseSettings>();
            mdb.Setup(db => db.Salt).Returns("y728sfLla98YUZpTgCM4VA==");

            //mock user repository
            var users = new List<User>();
            users.Add(new User() {
                Id = new Guid("35271bdf-250e-49ef-a89a-4bfc34408d2a"),
                Username = "test",
                Password = "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4=" //hashed version of "test"
            });
            users.Add(new User() {
                Id = new Guid("35271bdf-250e-49ef-a89a-4bfc34408d2b"),
                Username = "testtwo",
                Password = "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4=" //hashed version of "test"
            });
            var murepo = new Mock<IUserRepository>();
            murepo.Setup(repo => repo.GetUserByUsernameAndPassword(
                It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
                    (string u, string p) => users.Find(usr => usr.Username == u && usr.Password == p));
            murepo.Setup(repo => repo.GetUserById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => users.Find(usr => usr.Id == id));
            murepo.Setup(repo => repo.GetAllUsers()).ReturnsAsync(users);

            _userService = new UserService(mdb.Object, murepo.Object);
        }

        [Fact]
        public async void Authenticate_IsValid_ReturnResponse() {
            //arrange
            AuthenticateRequest req = new AuthenticateRequest() {
                Username = "test",
                Password = "test"
            };

            //act
            var response = await _userService.Authenticate(req);

            //assert
            Assert.NotNull(response);
            Assert.Equal("test", response.Username);
            Assert.Equal("35271bdf-250e-49ef-a89a-4bfc34408d2a", response.Id.ToString());
        }

        [Fact]
        public async void Authenticate_InvalidPassword_ReturnNull() {
            //arrange
            AuthenticateRequest req = new AuthenticateRequest() {
                Username = "test",
                Password = "testt"
            };

            //act
            var response = await _userService.Authenticate(req);

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

            //act
            var response = await _userService.Authenticate(req);

            //assert
            Assert.Null(response);
        }

        [Fact]
        public async void GetById_ValidId_ReturnUser() {
            //arrange
            string id = "35271bdf-250e-49ef-a89a-4bfc34408d2a";

            //act
            var user = await _userService.GetById(id);

            //assert
            Assert.Equal("35271bdf-250e-49ef-a89a-4bfc34408d2a", user.Id.ToString());
            Assert.Equal("test", user.Username);
        }

        [Fact]
        public async void GetById_InvalidId_ReturnNull() {
            //arrange
            string id = "35271bdf-250e-49ef-a89a-4bfc34408d2c";

            //act
            var user = await _userService.GetById(id);

            //assert
            Assert.Null(user);
        }

        [Fact]
        public async void GetAll_ReturnAll() {
            //arrange
            //act
            var users = await _userService.GetAll();

            //assert
            Assert.Equal(2, users.Count);
        }
    }
}