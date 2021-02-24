using Catflip.Api;
using Catflip.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public Task CreateUser([FromBody] CreateUserDto user)
        {
            return UserRepository.CreateUser(user.Username, user.Password);
        }

        [HttpPut("{username}/changepassword")]
        public Task PatchUser([FromRoute]string username, [FromBody] ChangePasswordDto user)
        {
            return UserRepository.ChangePassword(username, user.OldPassword, user.NewPassword);
        }

        [HttpDelete("username")]
        public Task DeleteUser([FromBody] CreateUserDto user)
        {
            return Task.CompletedTask;
        }
    }
}
