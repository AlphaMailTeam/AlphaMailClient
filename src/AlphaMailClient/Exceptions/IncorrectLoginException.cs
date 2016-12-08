using System;

namespace AlphaMailClient.Exceptions
{
    public class IncorrectLoginException : Exception
    {
        public string User { get; private set; }

        public IncorrectLoginException(string user)
        {
            User = user;
        }
    }
}

