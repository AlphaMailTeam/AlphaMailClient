using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading;

using AlphaMailClient.Cryptography;

namespace AlphaMailClient.AlphaMailClient
{
    public class AlphaMailClient
    {
        private KeyPair keys;
        private TcpClient client;
        private BinaryReader reader;
        private BinaryWriter writer;

        public AlphaMailClient(string host, int port, KeyPair keys)
        {
            this.keys = keys;
            client = new TcpClient(host, port);
            while (!client.Connected)
                ;
            reader = new BinaryReader(client.GetStream());
            writer = new BinaryWriter(client.GetStream());
        }

        public AlphaMailMessage[] CheckForMessages()
        {
            List<AlphaMailMessage> result = new List<AlphaMailMessage>();

            send("CHECK");

            string msg;
            while ((msg = readLine()).Trim().ToUpper() != "NOMOREMESSAGES")
            {
                string[] parts = msg.Split(' ');
                byte[] content = new JaCryptPkc().Decrypt(Convert.FromBase64String(parts[3]), keys.PublicKey, keys.PrivateKey);
                result.Add(new AlphaMailMessage(parts[1], parts[2], content));
            }

            return result.ToArray();
        }

        public AuthResultCode Login(string username, string password)
        {
            send("LOGIN {0} {1}", username, password);
            return (AuthResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        public AuthResultCode Register(string username, string password)
        {
            send("REGISTER {0} {1} {2} {3}", username, password, keys.PublicKey.Key, keys.PublicKey.E);
            return (AuthResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        public MessageResultCode SendMessage(string to, string message)
        {
            return SendMessage(to, ASCIIEncoding.ASCII.GetBytes(message));
        }
        public MessageResultCode SendMessage(string to, byte[] content)
        {
            byte[] encrypted = new JaCryptPkc().Encrypt(content, getKey(to));
            send("SEND {0} {1}", to, Convert.ToBase64String(encrypted));
            return (MessageResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        private PublicKey getKey(string user)
        {
            send("GETKEY {0}", user);
            string[] parts = readLine().Split(' ');
            return new PublicKey(BigInteger.Parse(parts[3]), BigInteger.Parse(parts[4]));
        }

        private bool reading = false;
        private string readLine()
        {
            while (reading)
                ;
            try
            {
                reading = true;
                return reader.ReadString();
            }
            finally
            {
                reading = false;
            }
        }
        private bool sending = false;
        private void send(string msg, params object[] args)
        {
            while (sending)
                ;
            sending = true;
            writer.Write(string.Format(msg, args));
            writer.Flush();
            sending = false;
        }
    }
}