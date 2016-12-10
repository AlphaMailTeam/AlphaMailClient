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
        private TextWriter output;

        public ClientUI(AlphaMailConfig config)
        {
            this.config = config;
            if (config.MessageFile != string.Empty && config.MessageFile != null)
            {
                var stream = new FileStream(config.MessageFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                stream.Position = stream.Length == 0 ? stream.Length : stream.Length - 1;
                output = new StreamWriter(stream);
            }
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

        public void CheckMail()
        {
            var mail = client.CheckForMessages();
            if (mail.Length <= 0)
                Console.WriteLine("No new messages!");
            foreach (var message in mail)
            {
                dualWrite(string.Format("From: {0}", message.Sender));
                dualWrite(string.Format("To: {0}", message.Recipient));
                dualWrite(string.Format("Content:\n{0}", message.MessageString));
            }
        }

        public void SendMessage(string to, byte[] content)
        {
            switch (client.SendMessage(to, content))
            {
                case MessageResultCode.MessageSuccess:
                    Console.WriteLine("Message successfully sent!");
                    break;
                case MessageResultCode.NoUser:
                    Console.WriteLine("Could not send, no such user, {0}", to);
                    break;
            }
        }
        public void SendMessage(string to, string content)
        {
            client.SendMessage(to, content);
        }

        private void dualWrite(string line)
        {
            if (output != null)
            {
                output.WriteLine(line);
                output.Flush();
            }

            Console.WriteLine(line);
            Console.Out.Flush();
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

