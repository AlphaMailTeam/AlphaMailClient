using System;

namespace AlphaMailClient.Exceptions
{
    public class UserAlreadyRegisteredException : Exception
    {
        public string User { get; private set; }

        public UserAlreadyRegisteredException(string user)
        {
            User = user;
        }
    }
}

