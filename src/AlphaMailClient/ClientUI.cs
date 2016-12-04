using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

using AlphaMailClient.Cryptography;
using AlphaMailClient.Events;

namespace AlphaMailClient
{
    public class ClientUI
    {
        private JaCryptPkc pkc = new JaCryptPkc();
        private Client client;
        private AlphaMailConfig config;

        private Dictionary<string, PublicKey> keys = new Dictionary<string, PublicKey>();

        public ClientUI(AlphaMailConfig config)
        {
            this.config = config;

            client = new Client(config.Server, config.Port);
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

            client.SendRegister(config.Username, config.Password, config.KeyPair.PublicKey.Key.ToString(), config.KeyPair.PublicKey.E.ToString());
            Thread.Sleep(1000);
            client.SendLogin(config.Username, config.Password);

            while (true)
            {
                string command = Console.ReadLine();
                string[] parts = command.Split(' ');

                switch (parts[0].ToUpper())
                {
                    case "CHECK":
                        client.SendCheck();
                        break;
                    case "SEND":
                        Console.Write("To: ");
                        string to = Console.ReadLine();
                        client.SendGetKey(to);
                        Console.WriteLine("Use file y/anyKey: ");
                        bool useFile = Console.ReadLine().Trim().ToLower() == "y";
                        byte[] content;
                        if (useFile)
                            content = File.ReadAllBytes(Console.ReadLine());
                        else
                        {
                            Console.Write("Message: ");
                            content = ASCIIEncoding.ASCII.GetBytes(Console.ReadLine());
                        }

                        if (keys.ContainsKey(to))
                        {
                            content = pkc.Encrypt(content, keys[to]);
                            client.SendMessage(to, Convert.ToBase64String(content));
                        }
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
            if (keys.ContainsKey(e.User))
                keys.Remove(e.User);
            keys.Add(e.User, e.PublicKey);
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

