using System;
using System.Text;

using AlphaMailClient.Cryptography;

namespace AlphaMailClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            /*
            var publicKey = PublicKey.FromFile(args[1]);
            var privateKey = PrivateKey.FromFile(args[2]);
            Console.WriteLine("Config Public Key:\n{0}", config.KeyPair.PublicKey.Key);
            Console.WriteLine("PublicKey Key:\n{0}", publicKey.Key);

            Console.WriteLine("Config e:\n{0}", config.KeyPair.PublicKey.E);
            Console.WriteLine("PublicKey e:\n{0}", publicKey.E);

            Console.WriteLine("Config private key:\n{0}", config.KeyPair.PrivateKey.Key);
            Console.WriteLine("PrivateKey key:\n{0}", privateKey.Key);
*/
            /*var config = AlphaMailConfig.FromFile(args[0]);
            var str = "Hello";
            var encrypted = new JaCryptPkc().Encrypt(ASCIIEncoding.ASCII.GetBytes(str), config.KeyPair.PublicKey);
            var decrypted = new JaCryptPkc().Decrypt(encrypted, config.KeyPair.PublicKey, config.KeyPair.PrivateKey);
            Console.WriteLine(ASCIIEncoding.ASCII.GetString(decrypted));*/
            new AlphaMailClientConfigParser().Parse(args).Execute();
        }
    }
}
