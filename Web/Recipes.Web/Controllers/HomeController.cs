namespace Recipes.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using Recipes.Services.Data;
    using Recipes.Web.ViewModels;
    using Recipes.Web.ViewModels.Home;

    public class HomeController : BaseController
    {
        private readonly IGetCountService countService;
        private readonly IRecipeService recipeService;

        public HomeController(IGetCountService countService, IRecipeService recipeService)
        {
            this.countService = countService;
            this.recipeService = recipeService;
        }

        public IActionResult Index()
        {
            var model = this.countService.GetCounts();
            model.RandomRecipes = this.recipeService
                .GetRandom<IndexPageRecipeViewModel>(3);
            // На GetRandom му се подава темплейтно модела към който ауто мапера да мапне от Recipe.

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
