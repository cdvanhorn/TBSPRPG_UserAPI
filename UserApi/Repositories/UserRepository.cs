using MongoDB.Driver;

using System.Collections.Generic;
using System.Threading.Tasks;

using UserApi.Entities;

using TbspRpgLib.Repositories;
using TbspRpgLib.Settings;

namespace UserApi.Repositories {
    public interface IUserRepository {
        Task<User> GetUserById(string id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserByUsernameAndPassword(string username, string password);
    }

    public class UserRepository : MongoRepository, IUserRepository{
        private readonly IMongoCollection<User> _users;

        public UserRepository(IDatabaseSettings databaseSettings) : base(databaseSettings){
            _users = _mongoDatabase.GetCollection<User>("users");
        }

        public Task<User> GetUserById(string id) {
            return _users.Find<User>(u => u.Id == id).FirstOrDefaultAsync();
        }

        public Task<User> GetUserByUsernameAndPassword(string username, string password) {
            return _users.Find<User>(user => user.Username == username && user.Password == password).FirstOrDefaultAsync();
        }

        public Task<List<User>> GetAllUsers() {
            return _users.Find(user => true).ToListAsync();
        }
    }
}