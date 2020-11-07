using Moq;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

using UserApi.Entities;
using UserApi.Models;
using UserApi.Services;
using UserApi.Repositories;
using UserApi.Utilities;

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
                Id = "8675309",
                Username = "test",
                Password = "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4=" //hashed version of "test"
            });
            users.Add(new User() {
                Id = "8675310",
                Username = "testtwo",
                Password = "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4=" //hashed version of "test"
            });
            var murepo = new Mock<IUserRepository>();
            murepo.Setup(repo => repo.GetUserByUsernameAndPassword(
                It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
                    (string u, string p) => users.Find(usr => usr.Username == u && usr.Password == p));
            murepo.Setup(repo => repo.GetUserById(It.IsAny<string>()))
                .ReturnsAsync((string id) => users.Find(usr => usr.Id == id));
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
            Assert.Equal("8675309", response.Id);
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
            string id = "8675309";

            //act
            var user = await _userService.GetById(id);

            //assert
            Assert.Equal("8675309", user.Id);
            Assert.Equal("test", user.Username);
        }

        [Fact]
        public async void GetById_InvalidId_ReturnNull() {
            //arrange
            string id = "8675308";

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