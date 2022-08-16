using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using HttpHandler;

namespace NUnitTest_Web.Pages.Test
{
    public class HttpHandlerTestModel : PageModel
    {
        //private readonly IHttpContextAccessor _httpContextAccessor;
        //public HttpHandlerTestModel(IHttpContextAccessor httpContextAccessor)
        //{
        //    _httpContextAccessor = httpContextAccessor;
        //}

        //public HttpHandlerTestModel(Microsoft.AspNetCore.Http.HttpContext httpcontext)
        //{

        //}
        public void OnGet()
        {
            //RequestUtility.HttpContext.Configure(_httpContextAccessor);

            //var request = HttpContext.Request;
                
            //var a = RequestUtility.Url.Host;
        }
    }
}
