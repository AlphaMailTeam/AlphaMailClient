using System;
using System.IO;
using System.Text;

using AlphaMailClient.AlphaMailClient;

namespace AlphaMailClient
{
    public class AlphaMailClientConfig
    {
        public string AlphaMailConfigFilePath { get; set; }
        public AlphaMailClientMode AlphaMailClientMode { get; set; }

        public string ToUser { get; set; }
        public byte[] Message { get; set; }

        public AlphaMailClientConfig()
        {
            AlphaMailClientMode = AlphaMailClientMode.None;
        }

        public void Execute()
        {
            verifyConfig();

            ClientUI client = new ClientUI(AlphaMailConfig.FromFile(AlphaMailConfigFilePath));
            client.Start();

            switch (AlphaMailClientMode)
            {
                case AlphaMailClientMode.Check:
                    client.CheckMail();
                    break;
                case AlphaMailClientMode.Send:
                    client.SendMessage(ToUser, Message);
                    break;
            }
        }

        private void verifyConfig()
        {
            if (AlphaMailConfigFilePath == string.Empty || AlphaMailConfigFilePath == null)
                die("Must specify config! Run --help for help.");
            switch (AlphaMailClientMode)
            {
                case AlphaMailClientMode.None:
                    die("Must specify mode with --check or --send. Run --help for help.");
                    break;
                case AlphaMailClientMode.Send:
                    if (ToUser == string.Empty || ToUser == null)
                        ToUser = promptForString("Enter recipient username: ");
                    if (Message == null)
                        Message = ASCIIEncoding.ASCII.GetBytes(promptForString("Enter the message: "));
                    break;
            }
        }

        private string promptForString(string msg, params string[] args)
        {
            Console.Write(msg, args);
            return Console.ReadLine();
        }

        private void die(string msg = "", params string[] args)
        {
            Console.WriteLine(msg, args);
            Environment.Exit(0);
        }
    }

    public enum AlphaMailClientMode
    {
        Check,
        None,
        Send
    }
}

