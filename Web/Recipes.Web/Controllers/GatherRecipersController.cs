namespace Recipes.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Recipes.Services;

    public class GatherRecipersController : BaseController
    {
        private readonly IGotvachBgScrapperService scrService;

        public GatherRecipersController(IGotvachBgScrapperService scrService)
        {
            this.scrService = scrService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Add()
        {
            await this.scrService.PopulateDbWithRecipesAsync(100);

            return this.RedirectToAction("Index");
        }
    }
}
