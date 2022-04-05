using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaeAdministrationTool.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CAEAdminServices _caeAdminServices;

        public IndexModel(CAEAdminServices caeAdminServices)
        {
            _caeAdminServices = caeAdminServices;
        }

        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}