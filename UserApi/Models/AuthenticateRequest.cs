using System.ComponentModel.DataAnnotations;

namespace UserApi.Models {
    public class AuthenticateRequest {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}