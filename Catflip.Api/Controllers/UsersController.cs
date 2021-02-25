using Catflip.Api;
using Catflip.Api.Dtos;
using Catflip.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Catflip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserRepository UserRepository { get; }

        public UsersController(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <remarks>
        /// The password must contain at least 1 number and 1 special character form '#, ?, !, @, $, %, ^, &amp;, *, -' and be at least 8 characters long.
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task CreateUser([FromBody] CreateUserDto user)
        {
            return UserRepository.CreateUser(user.Username, user.Password);
        }

        /// <summary>
        /// Update the users password
        /// </summary>
        /// <remarks>
        /// The password must contain at least 1 number and 1 special character form '#, ?, !, @, $, %, ^, &amp;, *, -' and be at least 8 characters long.
        /// </remarks>
        /// <param name="username">The user to update</param>
        [HttpPut("{username}/changepassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public Task PatchUser([FromRoute]string username, [FromBody] ChangePasswordDto user)
        {
            return UserRepository.ChangePassword(username, user.OldPassword, user.NewPassword);
        }

        /// <summary>
        /// Deletes a user from the application
        /// </summary>
        /// <remarks>
        /// You can only delete your own account.
        /// </remarks>
        /// <param name="username">The user to delete</param>
        [HttpDelete("{username}"), Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser([FromRoute] string username)
        {
            var loggedInUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            //Can only delete yourself
            if(loggedInUser == null || loggedInUser.Value != username)
            {
                return Forbid();
            }
            await UserRepository.DeleteUser(username);
            return Ok();
        }
    }
}
