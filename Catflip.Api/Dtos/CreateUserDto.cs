using System.ComponentModel.DataAnnotations;

namespace Catflip.Api.Dtos
{
    public class CreateUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required, StringLength(maximumLength: 255, MinimumLength = 8), PasswordComplexity]
        public string Password { get; set; }
    }
}
