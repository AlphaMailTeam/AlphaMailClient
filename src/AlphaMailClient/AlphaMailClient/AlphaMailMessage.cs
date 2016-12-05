using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlphaMailClient.AlphaMailClient
{
    public class AlphaMailMessage
    {
        public string Recipient { get; set; }
        public byte[] Message { get; set; }
        public string MessageInBase64 {  get { return Convert.ToBase64String(Message); } }

        public AlphaMailMessage()
        {
        }
        public AlphaMailMessage(string recipient)
        {
            Recipient = recipient;
        }
        public AlphaMailMessage(string recipient, byte[] message)
        {
            Recipient = recipient;
            Message = message;
        }
        public AlphaMailMessage(string recipient, string message)
        {
            Recipient = recipient;
            Message = ASCIIEncoding.ASCII.GetBytes(message);
        }

        public void ChangeRecipient(string newRecipient)
        {
            Recipient = newRecipient;
        }

        public void ChangeMessage(byte[] message)
        {
            Message = message;
        }
        public void ChangeMessage(string message)
        {
            Message = ASCIIEncoding.ASCII.GetBytes(message);
        }
    }
}
