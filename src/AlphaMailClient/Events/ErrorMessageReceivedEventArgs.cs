using System;

namespace AlphaMailClient.Events
{
    public class ErrorMessageReceivedEventArgs : MessageReceivedEventArgs
    {
        public string Error { get; private set; }

        public ErrorMessageReceivedEventArgs(string rawMessage, string error) : base(rawMessage)
        {
            Error = error;
        }
    }
}

