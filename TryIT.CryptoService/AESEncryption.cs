using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TryIT.CryptoService
{
    /// <summary>
    /// perform Encryption or Decryption with AES, Advanced Encryption Standard (Rijndael)
    /// </summary>
    public class AESEncryption
    {
        #region AES, Advanced Encryption Standard (Rijndael)
        /// <summary>
        /// AES Decrypt, return empty if cipherText not valid Base64String
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="password"></param>
        /// <param name="isReFormat">ReFormat string for URL, because standard Base64 may contain '/','+' it will show %xx in Url</param>
        /// <returns></returns>
        public static string AESDecrypt(string cipherText, string password, bool isReFormat = false)
        {
            string value = string.Empty;
            try
            {
                if (isReFormat)
                {
                    cipherText = UrlHelper.UrlBeauty_D(cipherText);
                }
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] decryptedData = EncryptOrDecrypt(cipherBytes, password, false);
                value = Encoding.Unicode.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;
        }

        /// <summary>
        /// AES Encrypt
        /// </summary>
        /// <param name="clearText"></param>
        /// <param name="password"></param>
        /// <param name="isReFormat"></param>
        /// <returns></returns>
        public static string AESEncrypt(string clearText, string password, bool isReFormat = false)
        {
            byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
            byte[] encryptedData = EncryptOrDecrypt(clearBytes, password, true);
            string result = Convert.ToBase64String(encryptedData);
            if (isReFormat)
            {
                result = UrlHelper.UrlBeauty_E(result);
            }
            return result;
        }

        private static byte[] EncryptOrDecrypt(byte[] data, string password, bool isEncrypt)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Rijndael alg = Rijndael.Create())
                {
                    PasswordDeriveBytes pdb = GetPasswordBytes(password);
                    alg.Key = pdb.GetBytes(32);
                    alg.IV = pdb.GetBytes(16);

                    using (ICryptoTransform transform = isEncrypt ? alg.CreateEncryptor() : alg.CreateDecryptor())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, transform, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.Close();
                        }
                        byte[] decryptedData = ms.ToArray();
                        return decryptedData;
                    }
                }
            }
        }

        private static PasswordDeriveBytes GetPasswordBytes(string password)
        {
            return new PasswordDeriveBytes(password, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        }
        #endregion
    }
}
