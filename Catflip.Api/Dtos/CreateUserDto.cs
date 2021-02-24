using System.ComponentModel.DataAnnotations;

namespace Catflip.Api
{
    public class CreateUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required, StringLength(maximumLength:255, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
