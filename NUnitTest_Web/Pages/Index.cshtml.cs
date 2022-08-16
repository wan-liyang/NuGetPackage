using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SqlAdoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NUnitTest_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private AdoConfig _config;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            //_config = sqlAdoConfig;
        }

        public void OnGet()
        {
            //SqlAdoUtility.ExecuteNonQuery("", System.Data.CommandType.Text);

        }
    }
}
