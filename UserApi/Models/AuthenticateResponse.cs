using UserApi.Entities;

namespace UserApi.Models
{
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }

        public AuthenticateResponse(User user)
        {
            Id = user.Id;
            Username = user.Username;
        }
    }
}