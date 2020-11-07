using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UserApi.Entities;
using UserApi.Models;
using UserApi.Repositories;
using UserApi.Utilities;

namespace UserApi.Services {
    public interface IUserService {
        Task<User> GetById(string id);
        Task<List<User>> GetAll();
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    }

    public class UserService : IUserService {
        private readonly IDatabaseSettings _databaseSettings;
        private IUserRepository _userRepository;

        public UserService(IDatabaseSettings databaseSettings, IUserRepository userRepository) {
            _databaseSettings = databaseSettings;
            _userRepository = userRepository;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            //we'll need to add the salt and hash the password
            //then check that against the database value
            string hashedPw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: model.Password,
                salt: Convert.FromBase64String(_databaseSettings.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            var user = await _userRepository.GetUserByUsernameAndPassword(model.Username, hashedPw);
        
            // return null if user not found
            if (user == null) return null;

            return new AuthenticateResponse(user);
        }

        public Task<User> GetById(string id) {
            return _userRepository.GetUserById(id);
        }

        public Task<List<User>> GetAll() {
            return _userRepository.GetAllUsers();
        }
    }
}