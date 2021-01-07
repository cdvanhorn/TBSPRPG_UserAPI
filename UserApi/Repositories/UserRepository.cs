using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

using Microsoft.EntityFrameworkCore;

using UserApi.Entities;

namespace UserApi.Repositories {
    public interface IUserRepository {
        Task<User> GetUserById(Guid id);
        Task<List<User>> GetAllUsers();
        Task<User> GetUserByUsernameAndPassword(string username, string password);
    }

    public class UserRepository : IUserRepository {
        private UserContext _context;

        public UserRepository(UserContext context) {
            _context = context;
        }

        public Task<User> GetUserById(Guid id) {
            return _context.Users.AsQueryable().Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public Task<User> GetUserByUsernameAndPassword(string username, string password) {
            var users = from usr in _context.Users.AsQueryable()
                        where usr.Username == username
                        where usr.Password == password
                        select usr;
            return users.FirstOrDefaultAsync();
        }

        public Task<List<User>> GetAllUsers() {
            return _context.Users.AsQueryable().ToListAsync<User>();
        }
    }
}