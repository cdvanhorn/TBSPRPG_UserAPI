using UserApi.Entities;

namespace UserApi.ViewModels {
    public class UserViewModel {
        public int Id { get; set; }

        public string Username { get; set; }

        public UserViewModel() {}

        public UserViewModel(User user) {
            Id = user.Id;
            Username = user.Username;
        }
    }
}