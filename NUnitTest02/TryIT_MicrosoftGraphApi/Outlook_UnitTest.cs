using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryIT.MicrosoftGraphApi.Model;
using TryIT.MicrosoftGraphApi.Model.Outlook;
using TryIT.MicrosoftGraphApi.MsGraphApi;

namespace NUnitTest02.TryIT_MicrosoftGraphApi
{
    internal class Outlook_UnitTest
    {
        [Test]
        public void Group_Test()
        {
            MsGraphApiConfig config = new MsGraphApiConfig
            {
                Token = "eyJ0eXAiOiJKV1QiLCJub25jZSI6Ii1IdEJVQ0hIVDRNM2FlQUVhMlpCc0ppN3FKZjMwMXA2Tnc0R21CbDl5YmciLCJhbGciOiJSUzI1NiIsIng1dCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyIsImtpZCI6Ii1LSTNROW5OUjdiUm9meG1lWm9YcWJIWkdldyJ9.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9jYTkwZDhmNS04OTYzLTRiNmUtYmNhOS05YWM0NjhiY2M3YTgvIiwiaWF0IjoxNjkzOTk0MzcyLCJuYmYiOjE2OTM5OTQzNzIsImV4cCI6MTY5Mzk5OTU1OSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhVQUFBQUNUaGhYbENLb3JDYm52MU5DRUgzSGJmL2FYSVUwbmNWbWlMSDUvcXlFYldFd0VZVUJrWWYvbThubnlKeW9tUWtFRTJnbmkzM1NaUDVvOHRtY2J4blJNZ1FJdENXUXlJSVlQOTc3dkpvYmk4PSIsImFtciI6WyJ3aWEiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoibmNzaXQtZXRsLXVhdC1hcHAiLCJhcHBpZCI6IjMwZGQ2N2NjLTQ5MjMtNGQ4NS1hYzYyLWM5NzUzNWRjNTYzOCIsImFwcGlkYWNyIjoiMSIsImZhbWlseV9uYW1lIjoiTkNTSVQiLCJnaXZlbl9uYW1lIjoiRVRMIiwiaWR0eXAiOiJ1c2VyIiwiaW5fY29ycCI6InRydWUiLCJpcGFkZHIiOiIyMDMuMTI2LjEzNC4yMiIsIm5hbWUiOiJFVEwgTkNTSVQgIChOQ1MpIiwib2lkIjoiNDZmYmZjZjMtMTY1Yy00NWI5LThhNjMtYTI2YzQzNzFmODMxIiwib25wcmVtX3NpZCI6IlMtMS01LTIxLTEyODgwMzI5NS0zMjY5ODExNDMtMzU3NDU5Mjk2LTM1Mzc3NCIsInBsYXRmIjoiMyIsInB1aWQiOiIxMDAzMjAwMjM1MDVFRTA4IiwicmgiOiIwLkFWWUE5ZGlReW1PSmJrdThxWnJFYUx6SHFBTUFBQUFBQUFBQXdBQUFBQUFBQUFCV0FGRS4iLCJzY3AiOiJBcHBsaWNhdGlvbi5SZWFkLkFsbCBDaGFubmVsLkNyZWF0ZSBDaGFubmVsLlJlYWRCYXNpYy5BbGwgQ2hhbm5lbE1lbWJlci5SZWFkV3JpdGUuQWxsIGVtYWlsIEZpbGVzLlJlYWRXcml0ZSBGaWxlcy5SZWFkV3JpdGUuQWxsIEdyb3VwTWVtYmVyLlJlYWRXcml0ZS5BbGwgTWFpbC5SZWFkIE1haWwuU2VuZCBNYWlsLlNlbmQuU2hhcmVkIG9wZW5pZCBwcm9maWxlIFNpdGVzLlJlYWQuQWxsIFNpdGVzLlJlYWRXcml0ZS5BbGwgVGVhbS5DcmVhdGUgVGVhbS5SZWFkQmFzaWMuQWxsIFRlYW1NZW1iZXIuUmVhZFdyaXRlLkFsbCBVc2VyLlJlYWQgVXNlci5SZWFkLkFsbCIsInNpZ25pbl9zdGF0ZSI6WyJpbmtub3dubnR3ayJdLCJzdWIiOiJ2THpHWEQwZTBIX3JCczM1TGVwZHhaVHJxdjNhUlFPWFBWeHJVWDRWWFhjIiwidGVuYW50X3JlZ2lvbl9zY29wZSI6IkFTIiwidGlkIjoiY2E5MGQ4ZjUtODk2My00YjZlLWJjYTktOWFjNDY4YmNjN2E4IiwidW5pcXVlX25hbWUiOiJldGwuaW50ZWdyYXRpb24ubmNzaXRAbmNzLmNvbS5zZyIsInVwbiI6ImV0bC5pbnRlZ3JhdGlvbi5uY3NpdEBuY3MuY29tLnNnIiwidXRpIjoiSHVnWXRHRmU0MEtJTGdjb3g0QVRBQSIsInZlciI6IjEuMCIsIndpZHMiOlsiYjc5ZmJmNGQtM2VmOS00Njg5LTgxNDMtNzZiMTk0ZTg1NTA5Il0sInhtc19zdCI6eyJzdWIiOiJWMVZkOGw3cnlWaExaaVdiWFhmelVQbC1DQ2ZTYXpLWnFEOWRIWkdfaEw4In0sInhtc190Y2R0IjoxNDc0ODc0NDI2fQ.NPFWQr1aFFrspZNSUc5gnqaJRR9QxYX_pltNQVwyKV2mLjJKKyXtezJ5esS89bDa1iZRSBA_pzjBwiVCh6IwWY61x_XjQQ3BVA0NBsR-2MYfsIrPGQvxVyjWKM-RxtSqrrgD_TuI85gIewVhJlzDAcsxijoPdbQFtBweOXOQ106sqh0rURAi-RH7qlNxbuq_ujAuoi66ZEwiFuLCcT7Xkh5QP0h9EswrCE0Bi2Jm6RMei5Vhc50sixn1TSwxe5nG3-LKVnT66cM0XUmWBOcikPOhjwZG8VCSX9fwq9yrREr-sv3xqOHHpC5i-ZkwRGZ3DbJVSgLyPYSaFsxORci8rw",
            };

            OutlookApi api = new OutlookApi(config);

            api.SendMessage(new SendMessageModel
            {
                From = "it-powerapps-noreply@ncs.co",
                Subject = $"Test Email {DateTime.Now}",
                Body = "Test",
                BodyContentType = BodyContentType.Html,
                ToRecipients = "liyang.wan2@ncs.co".Split(','),
                CcRecipients = null,
                Attachments = null
            });

            Assert.True(1 == 1);
        }
    }
}
