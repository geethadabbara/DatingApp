using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class LoginUserDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public LoginUserDto(string username, string password)
        {
            UserName = username.ToLower();
            Password = password.ToLower();
        }
    }
}