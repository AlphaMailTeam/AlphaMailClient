using System;

namespace AlphaMailClient
{
    public class ServerErrorException : Exception
    {
        public new string Message { get; private set; }

        public ServerErrorException(string message)
        {
            Message = message;
        }
    }
}

