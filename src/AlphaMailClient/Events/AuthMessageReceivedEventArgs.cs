using System;

namespace AlphaMailClient.Events
{
    public class AuthMessageReceivedEventArgs : MessageReceivedEventArgs
    {
        public string User { get; private set; }

        public AuthMessageReceivedEventArgs(string rawMessage, string user) : base(rawMessage)
        {
            User = user;
        }
    }
}

