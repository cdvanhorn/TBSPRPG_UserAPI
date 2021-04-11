using System;
using System.Threading.Tasks;
using UserApi.Entities;
using UserApi.Repositories;
using Xunit;

namespace UserApi.Tests.Repositories
{
    public class UserRepositoryTests : InMemoryTest
    {
        #region Setup
        private Guid _userOneId;
        public UserRepositoryTests() : base("UserRepositoryTests")
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

        #region GetAllUsers
        [Fact]
        public async Task GetAllUsers_ReturnAll()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repository = new UserRepository(context);

            //act
            var users = await repository.GetAllUsers();

            //assert
            Assert.Equal(2, users.Count);
            Assert.Equal("test", users[0].Username);
            Assert.Equal("testTwo", users[1].Username);
        }
        #endregion

        #region GetUserById
        [Fact]
        public async Task GetUserById_Valid_ReturnOne()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repo = new UserRepository(context);
            
            //act
            var user = await repo.GetUserById(_userOneId);

            //assert
            Assert.NotNull(user);
            Assert.Equal(_userOneId, user.Id);
            Assert.Equal("test", user.Username);
        }
        
        [Fact]
        public async Task GetUserById_InValid_ReturnNone()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repo = new UserRepository(context);
            
            //act
            var user = await repo.GetUserById(Guid.NewGuid());

            //assert
            Assert.Null(user);
        }
        #endregion
        
        #region GetUserByUsernameAndPassword
        [Fact]
        public async Task GetUserByUsernameAndPassword_Valid_ReturnOne()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repo = new UserRepository(context);
            
            //act
            var user = await repo.GetUserByUsernameAndPassword(
                "test", "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4=");

            //assert
            Assert.NotNull(user);
            Assert.Equal(_userOneId, user.Id);
            Assert.Equal("test", user.Username);
        }
        
        [Fact]
        public async Task GetUserByUsernameAndPassword_InValidUsername_ReturnNone()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repo = new UserRepository(context);
            
            //act
            var user = await repo.GetUserByUsernameAndPassword(
                "tEsT", "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4=");

            //assert
            Assert.Null(user);
        }
        
        [Fact]
        public async Task GetUserByUsernameAndPassword_InValidPassword_ReturnNone()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repo = new UserRepository(context);
            
            //act
            var user = await repo.GetUserByUsernameAndPassword(
                "test", "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4");
        
            //assert
            Assert.Null(user);
        }
        
        [Fact]
        public async Task GetUserByUsernameAndPassword_InValidBoth_ReturnNone()
        {
            //arrange
            await using var context = new UserContext(_dbContextOptions);
            var repo = new UserRepository(context);
            
            //act
            var user = await repo.GetUserByUsernameAndPassword(
                "tESt", "g4XyaMMxqIwlm0gklTRldD3PrM/xYTDWmpvfyKc8Gi4");

            //assert
            Assert.Null(user);
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