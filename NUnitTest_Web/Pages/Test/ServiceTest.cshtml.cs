using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using HttpHandler;
using WindowService;

namespace NUnitTest_Web.Pages.Test
{
    public class ServiceTest : PageModel
    {
        public void OnGet()
        {
        }

        [BindProperty]
        public string ServiceName { get; set; }

        //public IActionResult OnPost()
        //{
        //    ServiceUtility.StopService(ServiceName);

        //    return Page();
        //}
        public IActionResult Stop()
        {
            ServiceUtility.StopService(ServiceName);

            return Page();
        }
        public IActionResult Start()
        {
            ServiceUtility.StartService(ServiceName);

            return Page();
        }
    }
}
