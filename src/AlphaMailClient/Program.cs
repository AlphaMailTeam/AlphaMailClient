using System;

namespace AlphaMailClient
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            new ClientUI(AlphaMailConfig.FromFile(args[0])).Start();
        }
    }
}
