using Catflip.Api;
using Catflip.Api.Exceptions;
using Catflip.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Catflip.Api.Dtos;

namespace Catflip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        const string InternalAuthenticationType = "Internal";
        IUserRepository UserRepository { get; }
        IDiscoveryDocumentService DiscoveryDocumentService { get; }

        public AuthController(IUserRepository userRepository, IDiscoveryDocumentService discoveryDocumentService)
        {
            UserRepository = userRepository;
            DiscoveryDocumentService = discoveryDocumentService;
        }

        /// <summary>
        /// Login with an external identity provider
        /// </summary>
        /// <param name="redirectUri">The uri to redirect to after login</param>
        [HttpGet("login")]
        public IActionResult LoginExternal([FromQuery] string redirectUri)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = string.IsNullOrEmpty(redirectUri) ? "/" : redirectUri,
            };
            return new ChallengeResult(OpenIdConnectDefaults.AuthenticationScheme, properties);
        }

        /// <summary>
        /// Logout. Logs out of the external identity provider if supported
        /// </summary>
        /// <param name="redirectUri">The uri to redirect to after logout</param>
        [HttpGet("logout")]
        public async Task<IActionResult> LogoutExternal([FromQuery] string redirectUri)
        {        
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var authenticationResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if(authenticationResult.Succeeded)
            {
                var authenticationType = authenticationResult.Principal.Identity.AuthenticationType;
                if (authenticationType != InternalAuthenticationType && await DiscoveryDocumentService.SupportsFederatedLogout())
                {
                    return SignOut(new AuthenticationProperties { RedirectUri = string.IsNullOrEmpty(redirectUri) ? "/" : redirectUri, }, OpenIdConnectDefaults.AuthenticationScheme);
                }
            }

            return Redirect(string.IsNullOrEmpty(redirectUri) ? "/" : redirectUri);
        }

        /// <summary>
        /// Login with a user name and password
        /// </summary>
        [HttpPost("login")]
        public async Task Login([FromBody] LoginDto login)
        {
            if (login == null)
            {
                throw new InvalidLoginException();
            }
            var user = await UserRepository.ValidatedUser(login.Username, login.Password);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username)
            };

            var identity = new ClaimsIdentity(claims, InternalAuthenticationType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        }

    }
}
