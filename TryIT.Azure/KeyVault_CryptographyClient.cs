using System;
using System.Collections.Generic;
using System.Text;
using Azure.Core;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;

namespace TryIT.Azure
{
    public class KeyVault_CryptographyClient
    {
        private CryptographyClient cryptoClient;
        internal KeyVault_CryptographyClient(CryptographyClient cryptoClient)
        {
            this.cryptoClient = cryptoClient;
        }

        /// <summary>
        /// do encryption, return byte value, when do decryption needs pass same byte value
        /// </summary>
        /// <param name="plaintext"></param>
        public byte[] Encrypt(string plaintext)
        {
            byte[] _byte = Encoding.UTF8.GetBytes(plaintext);

            return DoEncryption(_byte, EncryptionAlgorithm.RsaOaep256);
        }

        private byte[] DoEncryption(byte[] _byte, EncryptionAlgorithm algorithm)
        {
            // https://crypto.stackexchange.com/questions/42097/what-is-the-maximum-size-of-the-plaintext-message-for-rsa-oaep

            int maxLength = 0;
            if (algorithm == EncryptionAlgorithm.RsaOaep256)
            {
                maxLength = 189; // 190 - 1;
            }

            if (_byte.Length <= maxLength)
            {
                return cryptoClient.Encrypt(algorithm, _byte).Ciphertext;
            }
            else
            {
                int repeatTimes = _byte.Length / maxLength;
                if (_byte.Length % maxLength > 0)
                {
                    repeatTimes += 1;
                }

                List<byte> byteResult = new List<byte>();

                for (int i = 0; i < repeatTimes; i++)
                {
                    // i = 0: 189
                    // i = 1: 189 * 2

                    if ((i + 1) * maxLength < _byte.Length)
                    {
                        byte[] tempByte = new byte[maxLength];
                        for (int j = i * maxLength; j < (i * maxLength) + maxLength; j++)
                        {
                            tempByte[j] = _byte[j + i * maxLength];
                        }

                        var result = cryptoClient.Encrypt(algorithm, tempByte).Ciphertext;
                        byteResult.AddRange(result);
                    }
                    else
                    {
                        int pastLength = i * maxLength;
                        int remainLength = _byte.Length - pastLength;
                        byte[] tempByte = new byte[remainLength];
                        for (int j = 0; j < remainLength; j++)
                        {
                            tempByte[j] = _byte[j + pastLength];
                        }

                        var result = cryptoClient.Encrypt(algorithm, tempByte).Ciphertext;
                        byteResult.AddRange(result);
                    }
                }

                return byteResult.ToArray();
            }
        }

        /// <summary>
        /// do decryption, required byte value from Encrypt method, return byte value for convert to expected result
        /// </summary>
        /// <param name="cipherValue"></param>
        public byte[] Decrypt(byte[] cipherValue)
        {
            return DoDecryption(cipherValue, EncryptionAlgorithm.RsaOaep256);
        }

        private byte[] DoDecryption(byte[] _byte, EncryptionAlgorithm algorithm)
        {
            int batchLength = 0;
            if (algorithm == EncryptionAlgorithm.RsaOaep256)
            {
                batchLength = 256;
            }

            int repeatTimes = _byte.Length / batchLength;

            List<byte> byteResult = new List<byte>();

            for (int i = 0; i < repeatTimes; i++)
            {
                byte[] tmpByte = new byte[batchLength];

                for (int j = 0; j < batchLength; j++)
                {
                    tmpByte[j] = _byte[j + batchLength * i];
                }

                var result = cryptoClient.Decrypt(algorithm, tmpByte).Plaintext;

                byteResult.AddRange(result);
            }

            return byteResult.ToArray();
        }
    }
}