namespace Recipes.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Recipes.Data.Models;
    using Recipes.Services.Data;
    using Recipes.Web.ViewModels.Recipes;

    public class RecipesController : Controller
    {
        private readonly ICategoriesService categoriesService;
        private readonly IRecipeService recipeService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        public RecipesController(ICategoriesService categoriesService, IRecipeService recipeService, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)

            // IWebHostEnvironment environment - това ще ни даде пътя до wwwroot. Тука се ползва се оказа.
        {
            this.categoriesService = categoriesService;
            this.recipeService = recipeService;
            this.userManager = userManager;
            this.environment = environment;
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            var inputModel = this.recipeService.GetById<EditRecipeInputModel>(id);
            inputModel.CategoriesItems = this.categoriesService
                .GetAllAsKeyValuePairs();

            return this.View(inputModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(int id, EditRecipeInputModel recipe)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View();
            }

            await this.recipeService.UpdateAsync(id, recipe);

            return this.RedirectToAction(nameof(this.ById), new { id }); // new { id } (new { id  = id})- би трябвало горе в адрес бара да сложи Id-то.
        }

        [Authorize]
        public IActionResult Create()
        {
            var viewModel = new CreateRecipeInputModel();
            viewModel.CategoriesItems = this.categoriesService.GetAllAsKeyValuePairs();

            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateRecipeInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                input.CategoriesItems = this.categoriesService
                    .GetAllAsKeyValuePairs();

                return this.View(input);
            }

            var userId_claim = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            // Един вариант за взимане на userId - през клеймовете (от бисквитката). Така не бъркаме в базата данни за това.
            // Втори вариант - инжектираме се UserManager<ApplicationUser> и го взимаме през него.

            var user = await this.userManager.GetUserAsync(this.User);
            // var userId = this.userManager.GetUserId(this.User);

            var path = $"{this.environment.WebRootPath}/images";

            try
            {
                await this.recipeService.CreateAsync(input, user.Id, path);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                input.CategoriesItems = this.categoriesService
                    .GetAllAsKeyValuePairs();
                return this.View(input);
            }

            // Ползване на TempData:
            // Пренася нещо между две заявки. В случая ще пренесе съобщението до All и след това TempData ще се изчисти.
            this.TempData["Message"] = "Recipe added successfully.";

            return this.RedirectToAction("All");
        }

        public IActionResult All(int id = 1) // За пейджирането. Това ще бъде номера на страницата.
        {
            if (id < 1)
            {
                return this.NotFound();
            }

            const int ItemsPerPage = 12;

            var viewModel = new RecipesListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                RecipesCount = this.recipeService.GetCount(),
                Recipes = this.recipeService.GetAll<RecipeInListViewModel>(id, ItemsPerPage),
            };

            return this.View(viewModel);
        }

        public IActionResult ById(int id)
        {
            var viewModel = this.recipeService
                .GetById<SingleRecipeViewModel>(id);
            
            return this.View(viewModel);
        }
    }
}
