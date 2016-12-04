using System;

namespace AlphaMailClient.Events
{
    public class MessageMessageReceivedEventArgs : MessageReceivedEventArgs
    {
        public string FromUser { get; private set; }
        public string Content { get; private set; }

        public MessageMessageReceivedEventArgs(string rawMessage, string fromUser, string content) : base(rawMessage)
        {
            FromUser = fromUser;
            Content = content;
        }
    }
}

