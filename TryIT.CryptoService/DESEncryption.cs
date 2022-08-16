using System;
using System.IO;
using System.Security.Cryptography;

namespace TryIT.CryptoService
{
    /// <summary>
    /// Data Encryption Standard
    /// </summary>
    [Obsolete("DESEncryption is deprecated, please use AESEncryption instead.")]
    public class DESEncryption
    {
        //8 bytes randomly selected for both the Key and the Initialization Vector
        //the IV is used to encrypt the first block of text so that any repetitive 
        //patterns are not apparent
        static private byte[] KEY_64 = new byte[] { 42, 16, 93, 156, 78, 4, 218, 32 };
        static private byte[] IV_64 = new byte[] { 55, 103, 246, 79, 36, 99, 167, 3 };

        //24 byte or 192 bit key and IV for TripleDES
        static private byte[] KEY_192 = new byte[] {42, 16, 93, 156, 78, 4, 218, 32,
            15, 167, 44, 80, 26, 250, 155, 112,
            2, 94, 11, 204, 119, 35, 184, 197};

        //24 byte or 192 bit key and IV for TripleDES
        static private byte[] IV_192 = new byte[]  {55, 103, 246, 79, 36, 99, 167, 3,
            42, 5, 62, 83, 184, 7, 209, 13,
            145, 23, 200, 58, 173, 10, 121, 222};


        public static string DESEncrypt(string value)
        {
            if (value != "")
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);

                sw.Write(value);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                //convert back to a string
                return System.Convert.ToBase64String(ms.GetBuffer(), 0, int.Parse(ms.Length.ToString()));
            }
            return "";
        }

        public static string DESDecrypt(string value)
        {
            if (value != "")
            {
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                //convert from string to byte array
                byte[] buffer = System.Convert.FromBase64String(value);

                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            return "";
        }

        public static string TripleDESEncrypt(string value)
        {
            if (value != "")
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cs);

                sw.Write(value);
                sw.Flush();
                cs.FlushFinalBlock();
                ms.Flush();

                //convert back to a string
                return System.Convert.ToBase64String(ms.GetBuffer(), 0, int.Parse(ms.Length.ToString()));
            }
            return "";
        }

        public static string TripleDESDecrypt(string value)
        {
            if (value != "")
            {
                TripleDESCryptoServiceProvider cryptoProvider = new TripleDESCryptoServiceProvider();

                //convert from string to byte array
                byte[] buffer = System.Convert.FromBase64String(value);
                MemoryStream ms = new MemoryStream(buffer);
                CryptoStream cs = new CryptoStream(ms, cryptoProvider.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            return "";
        }
    }
}
