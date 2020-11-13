namespace Recipes.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Recipes.Services.Data;
    using Recipes.Web.ViewModels.Recipes;

    public class RecipesController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IRecipeService recipeService;

        public RecipesController(ICategoriesService categoriesService, IRecipeService recipeService)
        {
            this.categoriesService = categoriesService;
            this.recipeService = recipeService;
        }

        public IActionResult Create()
        {
            var viewModel = new CreateRecipeInputModel();
            viewModel.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRecipeInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.CategoriesItems = this.categoriesService
                    .GetAllAsKeyValuePairs();

                return this.View(input);
            }

            await this.recipeService.CreateAsync(input);

            return this.Redirect("/");
        }
    }
}
