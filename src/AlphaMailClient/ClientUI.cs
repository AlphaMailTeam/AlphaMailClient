using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using AlphaMailClient.AlphaMailClient;
using AlphaMailClient.Cryptography;
using AlphaMailClient.Exceptions;

namespace AlphaMailClient
{
    public class ClientUI
    {
        private AlphaMailConfig config;
        private AlphaMailClient.AlphaMailClient client;

        public ClientUI(AlphaMailConfig config)
        {
            this.config = config;
        }

        public void Start()
        {
            client = new AlphaMailClient.AlphaMailClient(config.Server, config.Port, config.KeyPair);
            AuthResultCode registerCode = client.Register(config.Username, config.Password);
            AuthResultCode loginCode = client.Login(config.Username, config.Password);

            if (registerCode == AuthResultCode.RegisterBadUser && loginCode == AuthResultCode.LoginBadUser)
                throw new UserAlreadyRegisteredException(config.Username);
            if (loginCode != AuthResultCode.LoginSuccess)
                throw new IncorrectLoginException(config.Username);
        }

        public void CheckMail(TextWriter output)
        {
            var mail = client.CheckForMessages();
            if (mail.Length <= 0)
                output.WriteLine("No new messages!");
            foreach (var message in mail)
            {
                output.WriteLine(string.Format("From: {0}", message.Sender));
                output.WriteLine(string.Format("To {0}", message.Recipient));
                output.WriteLine(string.Format("Content:\n{0}", message.MessageString));
            }
            output.Flush();
        }

        public void SendMessage(string to, byte[] content)
        {
            client.SendMessage(to, content);
        }
        public void SendMessage(string to, string content)
        {
            client.SendMessage(to, content);
        }


        private string splitArray(string[] arr, int startIndex, char sep = ' ')
        {
            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < arr.Length; i++)
                sb.AppendFormat("{0}{1}", arr[i], sep);
            return sb.ToString();
        }
    }
}

