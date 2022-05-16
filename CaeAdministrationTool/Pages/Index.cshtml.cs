using CaeAdministrationTool.CAE;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CaeAdministrationTool.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CAEAdminServices _caeAdminServices;

        [BindProperty]
        public bool? Created { get; set; } = false;

        public IndexModel(CAEAdminServices caeAdminServices)
        {
            _caeAdminServices = caeAdminServices;
        }

        public async Task OnGetAsync()
        {
            var contexts = await _caeAdminServices.GetAuthContextValuesViaGraph();
            if (contexts.Count > 0)
                Created = true;
        }

        public async Task OnPostAsync()
        {
            var contextsToCreate = _caeAdminServices.GetSupportedAuthContextValues();
            foreach(var context in contextsToCreate)
            {
                await _caeAdminServices.CreateAuthContextViaGraph(context.Key, context.Value);
            }

            Redirect("/");
        }
    }
}