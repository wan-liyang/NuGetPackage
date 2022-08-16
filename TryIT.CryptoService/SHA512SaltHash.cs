using System;
using System.Security.Cryptography;
using System.Text;

namespace TryIT.CryptoService
{
    /// <summary>
    /// SHA512 generate/verify hash
    /// </summary>
    public class SHA512SaltHash
    {
        /// <summary>
        /// Compute SHA512 hash value with default salt
        /// </summary>
        /// <param name="stringToHash"></param>
        /// <returns></returns>
        public static string SHA512SaltComputeHash(string stringToHash)
        {
            string salt = GenerateRandomSalt();
            return SHA512ComputeHash(salt + stringToHash);
        }

        /// <summary>
        /// Verify SHA512 hash value with default salt
        /// </summary>
        /// <param name="stringToVerify"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool SHA512SaltVerifyHash(string stringToVerify, string hash)
        {
            string salt = GenerateRandomSalt();
            return VerifySaltHash(stringToVerify, hash, salt);
        }

        #region SHA512SaltHash
        private static string SHA512Salt_DO_NOT_CHANGE = "9ce5755d2f50dee8bbfec1ff3e65848cff9d52eb895d728f4dbf3386b32e475f24f51616adc5c647d78f6c7fcddbd576c8ee5604890b40fbb6eead3ef9b3ccd2";
        private static string GenerateRandomSalt()
        {
            return SHA512Salt_DO_NOT_CHANGE;
        }
        private static string ComputeHash(HashAlgorithm hashAlgorithm, string stringToHash)
        {
            var results = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
            return BytesToHexadecimalString(results);
        }
        private static string BytesToHexadecimalString(byte[] bytesToConvert)
        {
            var hexaDecimalString = new StringBuilder();

            for (var i = 0; i < bytesToConvert.Length; i++)
            {
                hexaDecimalString.Append(bytesToConvert[i].ToString("x2"));
            }
            return hexaDecimalString.ToString();
        }

        private static bool VerifySaltHash(string stringToVerify, string hash, string salt)
        {
            var hashedString = SHA512ComputeHash(salt + stringToVerify);
            return AreTwoStringsEqual(hash, hashedString);
        }
        private static bool AreTwoStringsEqual(string hash, string hashedString)
        {
            return hash.Equals(hashedString, StringComparison.OrdinalIgnoreCase);
        }

        private static string SHA512ComputeHash(string stringToHash)
        {
            string results;
            using (var sha512Algorithm = new SHA512Managed())
            {
                results = ComputeHash(sha512Algorithm, stringToHash);
                DisposeAlgorithm(sha512Algorithm);
            }

            return results;
        }

        private static void DisposeAlgorithm(HashAlgorithm hashAlgorithm)
        {
            hashAlgorithm.Clear();
        }
        #endregion
    }
}
