using System;
using System.IO;

namespace AlphaMailClient
{
    public class AlphaMailClientConfigParser
    {
        private string[] args;
        private int position;

        public AlphaMailClientConfig Parse(string[] args)
        {
            if (args.Length <= 0)
                displayHelp();
            this.args = args;

            AlphaMailClientConfig config = new AlphaMailClientConfig();

            for (position = 0; position < args.Length; position++)
            {
                switch (args[position])
                {
                    case "-c":
                    case "--check":
                        config.AlphaMailClientMode = AlphaMailClientMode.Check;
                        break;
                    case "-h":
                    case "--help":
                        displayHelp();
                        break;
                    case "-m":
                    case "--message":
                        config.Message = File.ReadAllBytes(expectData("[FILE]"));
                        break;
                    case "-r":
                    case "--recipient":
                        config.ToUser = expectData("[USER]");
                        break;
                    case "-s":
                    case "--send":
                        config.AlphaMailClientMode = AlphaMailClientMode.Send;
                        break;
                    default:
                        config.AlphaMailConfigFilePath = args[position];
                        break;
                }
            }
            return config;
        }

        private void displayHelp()
        {
            Console.WriteLine("Usage: AlphaMailClient.exe [CONFIG FILE] [MODE] [DATA]");
            Console.WriteLine("-h --help                     Displays this help and exits.");

            Console.WriteLine("\nModes:");
            Console.WriteLine("-c --check                    Enters check for new mail mode.");
            Console.WriteLine("-s --send                     Enters send mode with given recipient and message.");

            Console.WriteLine("\nData:");
            Console.WriteLine("-m --message [FILE]           Specifies the message from a file.");
            Console.WriteLine("-r --recipient [USER]         Specifies the desired recipient of a message.");
            die();
        }

        private string expectData(string type)
        {
            if (args[++position].StartsWith("-"))
                die("Expected data of type {0}, instead got flag {1}!", type, args[position]);
            return args[position];
        }

        private void die(string msg = "", params string[] args)
        {
            Console.WriteLine(msg, args);
            Environment.Exit(0);
        }
    }
}

