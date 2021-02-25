
using System.ComponentModel.DataAnnotations;

namespace Catflip.Api.Configuration
{
    public class OpenIdClientOptions
    {
        [Required]
        public string Authority { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ClientSecret { get; set; }
    }
}
