using NUnit.Framework;
using System;
using TryIT.HttpHandler;

namespace NUnitTest.HttpHandler
{
    class UrlBuilder_UnitTest
    {
        [Test]
        public void BuildUrl_Test()
        {
           string expectResult = "https://test/page.aspx?a=1&b=2";
           string url = new UrlBuilder()
               .Host("https://test/")
               .Url("~/page.aspx")
               .Parameter("a", "1")
               .Parameter("b", "2")
               .Encrypt(false)
               .ToString();

           Assert.AreEqual(expectResult, url);

           url = new UrlBuilder()
               .Host("https://test")
               .Url("~/page.aspx")
               .Parameter("a", "1")
               .Parameter("b", "2")
               .Encrypt(false)
               .ToString();

           Assert.AreEqual(expectResult, url);

           url = new UrlBuilder()
               .Host("https://test/")
               .Url("/page.aspx")
               .Parameter("a", "1")
               .Parameter("b", "2")
               .Encrypt(false)
               .ToString();

           Assert.AreEqual(expectResult, url);

           url = new UrlBuilder()
               .Host("https://test/")
               .Url("page.aspx")
               .Parameter("a", "1")
               .Parameter("b", "2")
               .Encrypt(false)
               .ToString();

           Assert.AreEqual(expectResult, url);
        }
    }
}
