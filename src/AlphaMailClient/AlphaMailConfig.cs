using System;
using System.IO;
using System.Numerics;
using System.Text;

using AlphaMailClient.Cryptography;

namespace AlphaMailClient
{
    public class AlphaMailConfig
    {
        public static AlphaMailConfig FromFile(string path)
        {
            StreamReader reader = new StreamReader(path);
            AlphaMailConfig config = new AlphaMailConfig();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                string line = readLine(reader.BaseStream);

                string[] parts = line.Split(' ');

                switch (parts[0].ToUpper())
                {
                    case "KEYPAIR":
                        config.KeyPair = new KeyPair(BigInteger.Parse(parts[1]), BigInteger.Parse(parts[2]), BigInteger.Parse(parts[3]));
                        break;
                    case "MESSAGEFILE":
                        config.MessageFile = parts[1];
                        break;
                    case "PASSWORD":
                        config.Password = parts[1];
                        break;
                    case "PORT":
                        config.Port = Convert.ToInt32(parts[1]);
                        break;
                    case "HOST":
                        config.Server = parts[1];
                        break;
                    case "USERNAME":
                        config.Username = parts[1];
                        break;
                }
            }
            reader.Close();
            return config;
        }

        private static string readLine(Stream stream)
        {
            StringBuilder sb = new StringBuilder();

            char c;

            while ((c = (char)stream.ReadByte()) != '\n')
                sb.Append(c);

            return sb.ToString();
        }

        public KeyPair KeyPair { get; set; }
        public string MessageFile { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Server { get; set; }
        public string Username { get; set; }
    }
}

