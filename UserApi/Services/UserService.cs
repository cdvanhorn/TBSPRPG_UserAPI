using Microsoft.AspNetCore.Cryptography.KeyDerivation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

using UserApi.Models;
using UserApi.Repositories;
using UserApi.ViewModels;

using TbspRpgLib.Settings;

namespace UserApi.Services {
    public interface IUserService {
        Task<UserViewModel> GetById(string id);
        Task<List<UserViewModel>> GetAll();
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

        public async Task<UserViewModel> GetById(string id) {
            Guid gid;
            if(!Guid.TryParse(id, out gid))
                return null;
            var user = await _userRepository.GetUserById(gid);
            if(user == null)
                return null;
            return new UserViewModel(user);
        }

        public async Task<List<UserViewModel>> GetAll() {
            var users = await _userRepository.GetAllUsers();
            return users.Select(usr => new UserViewModel(usr)).ToList();
        }
    }
}