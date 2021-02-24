using System.ComponentModel.DataAnnotations;

namespace Catflip.Api
{
    public class ChangePasswordDto
    {
        [Required, StringLength(maximumLength: 255, MinimumLength = 8)]
        public string NewPassword { get; set; }
        [Required, StringLength(maximumLength: 255, MinimumLength = 8)]
        public string OldPassword { get; set; }
    }
}
