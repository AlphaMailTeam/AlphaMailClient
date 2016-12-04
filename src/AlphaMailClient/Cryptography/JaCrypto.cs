using System;
using System.IO;

namespace JaCrypt.Cryptography
{
    public class JaCrypto
    {
        private Prng prng;

        public byte[] Encrypt(byte[] key, byte[] data)
        {
            uint seed = 0;
            prng = new Prng((uint)key.Length);
            for (int i = 0; i < key.Length; i++)
                seed += prng.NextByte((byte)(key[i] * i ^ seed));
            prng = new Prng(seed);

            byte[] result = new byte[data.Length];

            for (int i = 0; i < result.Length; i++)
            {
                byte b = prng.NextByte(key[i % key.Length]);
                key[i % key.Length] += b;
                result[i] = (byte)(0xFF - (b + data[i]));
            }

            return result;
        }
    }
}
