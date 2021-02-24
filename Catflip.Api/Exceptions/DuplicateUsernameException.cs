using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catflip.Api.Exceptions
{
    public class DuplicateUsernameException: Exception
    {
        public DuplicateUsernameException(string username):base($"The username {username} is already in use.")
        {

        }
    }
}
