using System;

namespace Catflip.Api.Exceptions
{
    public class InvalidLoginException: Exception
    {
        public InvalidLoginException(): base("The username or password is wrong.")
        {

        }
    }
}
