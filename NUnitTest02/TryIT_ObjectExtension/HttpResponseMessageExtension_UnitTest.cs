using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TryIT.ObjectExtension;

namespace NUnitTest02.TryIT_ObjectExtension
{
    internal class HttpResponseMessageExtension_UnitTest
    {
        [TestCase(HttpStatusCode.OK, "200 - OK")]
        [TestCase(HttpStatusCode.Created, "201 - Created")]
        [TestCase(HttpStatusCode.Accepted, "202 - Accepted")]
        [TestCase(HttpStatusCode.NoContent, "204 - NoContent")]
        [TestCase(HttpStatusCode.BadRequest, "400 - BadRequest")]
        [TestCase(HttpStatusCode.Unauthorized, "401 - Unauthorized")]
        [TestCase(HttpStatusCode.Forbidden, "403 - Forbidden")]
        [TestCase(HttpStatusCode.NotFound, "404 - NotFound")]
        [TestCase(HttpStatusCode.InternalServerError, "500 - InternalServerError")]
        [TestCase(HttpStatusCode.NotImplemented, "501 - NotImplemented")]
        [TestCase(HttpStatusCode.BadGateway, "502 - BadGateway")]
        [TestCase(HttpStatusCode.ServiceUnavailable, "503 - ServiceUnavailable")]
        public void GetStatusCode_ReturnsExpectedString_ForVariousStatusCodes(HttpStatusCode statusCode, string expected)
        {
            // Arrange
            var response = new HttpResponseMessage(statusCode);

            // Act
            var result = response.GetStatusCodeString();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetStatusCode_ThrowsArgumentNullException_WhenResponseIsNull()
        {
            // Arrange
            HttpResponseMessage response = null;

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => response.GetStatusCodeString());
            Assert.That(ex.Message, Does.Contain("Response cannot be null"));
        }
    }
}
