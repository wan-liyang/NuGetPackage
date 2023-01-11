using System;
using TryIT.DataConversion;
namespace NUnitTest02.TryIT_DataConversion
{
    public class ConvertString_UnitTest
    {
        [Test]
        public void ToDecimal_Test()
        {
            Assert.That("0".ToDecimal(), Is.EqualTo(0));
            Assert.That("1".ToDecimal(), Is.EqualTo(1));
            Assert.That("1.234".ToDecimal(), Is.EqualTo(1.234));
            Assert.That("1,234,567.234".ToDecimal(), Is.EqualTo(1234567.234));
            Assert.That("1,000,000".ToDecimal(), Is.EqualTo(1000000));
            Assert.That("1%".ToDecimal(), Is.EqualTo(0.01));


            Assert.That("-1.234".ToDecimal(), Is.EqualTo(-1.234));
            Assert.That("-1,234,567.234".ToDecimal(), Is.EqualTo(-1234567.234));

            Assert.That("".ToDecimal(), Is.EqualTo(null));
            Assert.That("-".ToDecimal(), Is.EqualTo(null));
        }
    }
}

