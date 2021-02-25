using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Catflip.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class ProfileController : ControllerBase
    {
        /// <summary>
        /// Returns all the claims for the current user
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ClaimDto[] UserProfile()
        {
            var claims = User.Claims.Select(c => new ClaimDto(c.Type, c.Value));
            return claims.ToArray() ;
        }
    }

    public record ClaimDto(string Type, string Value);
}
