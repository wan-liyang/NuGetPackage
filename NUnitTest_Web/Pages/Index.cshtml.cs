using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TryIT.SqlAdoService;

namespace NUnitTest_Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //SqlAdoUtility.ExecuteNonQuery("", System.Data.CommandType.Text);

        }
    }
}
