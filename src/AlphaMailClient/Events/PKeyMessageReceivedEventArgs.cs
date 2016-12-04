using System;
using System.Numerics;

using AlphaMailClient.Cryptography;

namespace AlphaMailClient.Events
{
    public class PKeyMessageReceivedEventArgs : MessageReceivedEventArgs
    {
        public string User { get; private set; }
        public PublicKey PublicKey { get; private set; }

        public PKeyMessageReceivedEventArgs(string rawMessage, string user, string pkey, string e) : base(rawMessage)
        {
            User = user;
            PublicKey = new PublicKey(BigInteger.Parse(pkey), BigInteger.Parse(e));
        }
    }
}

