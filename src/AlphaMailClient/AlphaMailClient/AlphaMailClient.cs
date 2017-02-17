using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using AlphaMailClient.Cryptography;

namespace AlphaMailClient.AlphaMailClient
{
    public class AlphaMailClient
    {
        public const string HASH_ALGO = "SHA512";

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
                byte[] content = new JaCryptPkc().Decrypt(Convert.FromBase64String(parts[4]), keys.PublicKey, keys.PrivateKey);
                byte[] subjectBytes = new JaCryptPkc().Decrypt(Convert.FromBase64String(parts[3]), keys.PublicKey, keys.PrivateKey);
                string subject = ASCIIEncoding.ASCII.GetString(subjectBytes);
                result.Add(new AlphaMailMessage(parts[1], subject, parts[2], content));
            }

            return result.ToArray();
        }

        public AuthResultCode Login(string username, string password)
        {
            send("AUTHKEY {0}", username);
            string randStr = readLine().Split(' ')[1];
            send("LOGIN {0} {1}", username, generateToken(hashString(password, HASH_ALGO), randStr));
            return (AuthResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        public AuthResultCode Register(string username, string password)
        {
            send("REGISTER {0} {1} {2} {3}", username, password, keys.PublicKey.Key, keys.PublicKey.E);
            return (AuthResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        public MessageResultCode SendMessage(string to, string subject, string message)
        {
            return SendMessage(to, ASCIIEncoding.ASCII.GetBytes(subject), ASCIIEncoding.ASCII.GetBytes(message));
        }
        public MessageResultCode SendMessage(string to, byte[] subject, byte[] content)
        {
            var key = getKey(to);
            byte[] encryptedContent = new JaCryptPkc().Encrypt(content, key);
            byte[] encryptedSubject = new JaCryptPkc().Encrypt(subject, key);
            send("SEND {0} {1} {2}", to, Convert.ToBase64String(encryptedSubject), Convert.ToBase64String(encryptedContent));
            return (MessageResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        public UpdateResultCode UpdateAccount(string username, string password, PublicKey key)
        {
            send("UPDATE {0} {1} {2} {3}", username, password, key.Key, key.E);
            return (UpdateResultCode)Convert.ToInt32(readLine().Split(' ')[1]);
        }

        private PublicKey getKey(string user)
        {
            send("GETKEY {0}", user);
            string[] parts = readLine().Split(' ');
            return new PublicKey(BigInteger.Parse(parts[3]), BigInteger.Parse(parts[4]));
        }

        private string generateToken(string passHash, string randChars)
        {
            return hashString(passHash + randChars, HASH_ALGO);
        }

        private string hashString(string text, string method)
        {
            return BitConverter.ToString(((HashAlgorithm)CryptoConfig.CreateFromName(method)).ComputeHash(new UTF8Encoding().GetBytes(text))).Replace("-", string.Empty).ToLower();
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