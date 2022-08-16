using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using CryptoService;

namespace NUnitTest.CryptoService
{
    class DESEncryption_UnitTest
    {
        [Test]
        public void DESEncrypt()
        {
            string value = DESEncryption.DESEncrypt("password1");
        }
        [Test]
        public void DESDecrypt()
        {
            
        }
    }
}
