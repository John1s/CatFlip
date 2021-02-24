
using System.ComponentModel.DataAnnotations;

namespace Catflip.Api.DataAccess
{
    public class UserEntity
    {
        [Key]
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }

}
