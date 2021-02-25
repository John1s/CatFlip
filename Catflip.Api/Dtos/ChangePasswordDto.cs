using System.ComponentModel.DataAnnotations;

namespace Catflip.Api.Dtos
{
    public class ChangePasswordDto
    {
        [Required, StringLength(maximumLength: 255, MinimumLength = 8), PasswordComplexity]
        public string NewPassword { get; set; }
        [Required]
        public string OldPassword { get; set; }
    }
}
