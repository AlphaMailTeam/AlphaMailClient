using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using AlphaMailClient.AlphaMailClient;
using AlphaMailClient.Cryptography;
using AlphaMailClient.Events;

namespace AlphaMailClient
{
    public class ClientUI
    {
        private JaCryptPkc pkc = new JaCryptPkc();
        private AlphaMailClient.AlphaMailClient client;
        private AlphaMailConfig config;

        public ClientUI(AlphaMailConfig config)
        {
            this.config = config;

            client = new AlphaMailClient.AlphaMailClient(config.Server, config.Port, config.Username, config.Password);
            client.AuthMessageReceived += client_AuthMessageReceived;
            client.ConnectedToServer += client_ConnectedToServer;
            client.DisconnectedFromServer += client_DisconnectedFromServer;
            client.ErrorMessageReceived += client_ErrorMessageReceived;
            client.MessageMessageReceived += client_MessageMessageReceived;
            client.PKeyMessageReceived += client_PKeyMessageReceivedEventArgs;
        }

        public void Start()
        {
            client.Start();

            while (true)
            {
                string command = Console.ReadLine();
                string[] parts = command.Split(' ');

                switch (parts[0].ToUpper())
                {
                    case "CHECK":
                        client.CheckForMessages();
                        break;
                    case "SEND":
                        bool clientExists;
                        string to;
                        PublicKey key;
                        do
                        {
                            Console.Write("To: ");
                            to = Console.ReadLine();
                            key = client.RequestEncryptionKey(to);
                            clientExists = key != null;
                        }
                        while (!clientExists);

                        Console.Write("Use file? y/any: ");
                        bool useFile = Console.ReadLine().Trim().ToLower() == "y";
                        byte[] content;
                        if (useFile)
                        {
                            Console.Write("File path: ");
                            content = File.ReadAllBytes(Console.ReadLine());
                        }
                        else
                            content = ASCIIEncoding.ASCII.GetBytes(Console.ReadLine());
                        content = pkc.Encrypt(content, key);

                        client.SendMessage(new AlphaMailMessage(to, content));
                        break;
                }
            }
        }

        private void client_AuthMessageReceived(object sender, AuthMessageReceivedEventArgs e)
        {
            Console.WriteLine("Authenticated for {0}!", e.User);
        }
        private void client_ConnectedToServer(object sender, ConnectedToServerEventArgs e)
        {
            Console.WriteLine("Connected to server! Use LOGIN/REGISTER commands to authenticate!");
        }
        private void client_DisconnectedFromServer(object sender, DisconnectedFromServerEventArgs e)
        {
            Console.WriteLine("Disconnected from server!");
            client.Close();
        }
        private void client_ErrorMessageReceived(object sender, ErrorMessageReceivedEventArgs e)
        {
            Console.WriteLine("Error: {0}", e.Error);
        }
        private void client_MessageMessageReceived(object sender, MessageMessageReceivedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Message Received!");
            sb.AppendFormat("From: {0}\n", e.FromUser);
            sb.AppendFormat("Content:\n{0}\n", ASCIIEncoding.ASCII.GetString(pkc.Decrypt(Convert.FromBase64String(e.Content), config.KeyPair.PublicKey, config.KeyPair.PrivateKey)));
            Console.WriteLine(sb.ToString());

            File.AppendAllText(config.MessageFile, sb.ToString());
        }
        private void client_PKeyMessageReceivedEventArgs(object sender, PKeyMessageReceivedEventArgs e)
        {
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

