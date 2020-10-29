using MongoDB.Driver;

using System.Collections.Generic;
using System.Threading.Tasks;

using UserApi.Entities;
using UserApi.Utilities;

namespace UserApi.Repositories {
    public interface IUserRepository {
        Task<User> GetUserById(string id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserByUsernameAndPassword(string username, string password);
    }

    public class UserRepository : IUserRepository{
        private readonly IDatabaseSettings _dbSettings;
        private readonly IMongoCollection<User> _users;

        public UserRepository(IDatabaseSettings databaseSettings) {
            _dbSettings = databaseSettings;

            var connectionString = $"mongodb+srv://{_dbSettings.Username}:{_dbSettings.Password}@{_dbSettings.Url}/{_dbSettings.Name}?retryWrites=true&w=majority";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(_dbSettings.Name);

            _users = database.GetCollection<User>("users");
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