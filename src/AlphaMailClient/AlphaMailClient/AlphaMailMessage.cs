using System;
using System.Collections.Generic;
using System.Text;

namespace AlphaMailClient.AlphaMailClient
{
    public class AlphaMailMessage
    {
        public byte[] Message { get; set; }
        public string MessageString { get { return ASCIIEncoding.ASCII.GetString(Message); } }
        public string MessageInBase64 { get { return Convert.ToBase64String(Message); } }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Subject { get; set; }
        public string SubjectInBase64 { get { return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(Subject)); } }

        public AlphaMailMessage()
        {
            Sender = string.Empty;
            Recipient = string.Empty;
            Subject = string.Empty;
            Message = new byte[0];
        }
        public AlphaMailMessage(string recipient)
        {
            Sender = string.Empty;
            Recipient = recipient;
            Subject = string.Empty;
            Message = new byte[0];
        }
        public AlphaMailMessage(string recipient, byte[] message)
        {
            Sender = string.Empty;
            Recipient = recipient;
            Subject = string.Empty;
            Message = message;
        }
        public AlphaMailMessage(string recipient, string message)
        {
            Sender = string.Empty;
            Recipient = recipient;
            Subject = string.Empty;
            Message = ASCIIEncoding.ASCII.GetBytes(message);
        }
        public AlphaMailMessage(string sender, string recipient, byte[] message)
        {
            Sender = sender;
            Recipient = recipient;
            Subject = string.Empty;
            Message = message;
        }
        public AlphaMailMessage(string sender, string recipient, string message)
        {
            Sender = sender;
            Recipient = recipient;
            Subject = string.Empty;
            Message = ASCIIEncoding.ASCII.GetBytes(message);
        }
        public AlphaMailMessage(string sender, string subject, string recipient, byte[] message)
        {
            Sender = sender;
            Recipient = recipient;
            Subject = subject;
            Message = message;
        }
        public AlphaMailMessage(string sender, string subject, string recipient, string message)
        {
            Sender = sender;
            Recipient = recipient;
            Subject = subject;
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
