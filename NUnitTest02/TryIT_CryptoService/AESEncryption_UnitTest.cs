using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TryIT.CryptoService;

namespace NUnitTest.CryptoService
{
    class AESEncryption_UnitTest
    {
        [Test]
        public void AESEncrypt()
        {
            string clearText = "test text";
            string password = "password";
            string _value = "Ia6zEst27Xy5vjjekQhnD5HUmLcRFtj9zdINu04LNW4=";

            string ciphervalue = AESEncryption.AESEncrypt(clearText, password);

            Assert.AreEqual(_value, ciphervalue);

            string clearvalue = AESEncryption.AESDecrypt(ciphervalue, password);

            Assert.AreEqual(clearText, clearvalue);
        }
    }
}
