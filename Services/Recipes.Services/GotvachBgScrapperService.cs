namespace Recipes.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using AngleSharp;
    using Recipes.Data.Common.Repositories;
    using Recipes.Data.Models;
    using Recipes.Services.Models;

    public class GotvachBgScrapperService : IGotvachBgScrapperService
    {
        // AngleSharp library. Менажира Dom дървото.
        private readonly IConfiguration config;
        private readonly IBrowsingContext context;

        private readonly IDeletableEntityRepository<Category> categoryRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientRepository;

        public GotvachBgScrapperService(IDeletableEntityRepository<Category> categoryRepository, IDeletableEntityRepository<Ingredient> ingredientRepository)
        {
            this.config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(this.config);

            this.categoryRepository = categoryRepository;
            this.ingredientRepository = ingredientRepository;
        }

        public async Task PopulateDbWithRecipes()
        {
            var concurentBag = new ConcurrentBag<RecipeDto>();

            Parallel.For(1, 300, (i) =>
            {
                try
                {
                    var recipe = this.GetRecipe(i);
                    concurentBag.Add(recipe);
                }
                catch { }
            });

            foreach (var recipe in concurentBag)
            {
                var categoryId = await this.GetOrCreateCategoryAsync(recipe.CategoryName);
                var ingredientIds = await this.GetOrCreateIngredientAsync(recipe.IngredientsName);
            }
        }

        private async Task<IEnumerable<int>> GetOrCreateIngredientAsync(List<string> ingredientsName)
        {
            foreach (var ingredientName in ingredientsName)
            {

            }
        }

        private async Task<int> GetOrCreateCategoryAsync(string categoryName)
        {
            var category = this.categoryRepository.AllAsNoTracking()
                .FirstOrDefault(x => x.Name == categoryName);

            if (category == null)
            {
                category = new Category
                {
                    Name = categoryName,
                };

                await this.categoryRepository.AddAsync(category);
            }

            return category.Id;
        }

        private RecipeDto GetRecipe(int id)
        {
            var document = this.context
                .OpenAsync($"https://recepti.gotvach.bg/r-{id}")
                .GetAwaiter().GetResult();

            if (document.StatusCode == HttpStatusCode.NotFound)
            {
                throw new InvalidOperationException();
            }

            var recipe = new RecipeDto();

            var recipeNameAndCategory = document
                .QuerySelectorAll("#recEntity > div.breadcrumb")
                .Select(x => x.TextContent).FirstOrDefault()
                .Split(" »", StringSplitOptions.RemoveEmptyEntries)
                .Reverse().ToArray();

            recipe.CategoryName = recipeNameAndCategory[1];
            // Console.WriteLine(categoryName);

            recipe.RecipeName = recipeNameAndCategory[0].TrimStart();
            // Console.WriteLine(recipeName);

            var instructions = document.QuerySelectorAll(".text > p")
                .Select(x => x.TextContent).ToList();

            var sb = new StringBuilder();

            foreach (var item in instructions)
            {
                sb.AppendLine(item);
            }

            recipe.Instructions = sb.ToString().TrimEnd();

            var timing = document.QuerySelectorAll(".mbox > .feat.small");

            if (timing.Length > 0)
            {
                var prepTime = timing[0].TextContent
                .Replace("Приготвяне", string.Empty)
                .Replace(" мин.", string.Empty);

                recipe.PreparationTime = TimeSpan.ParseExact(prepTime, "mm", CultureInfo.InvariantCulture);
                // Console.WriteLine(prepTime);
            }

            if (timing.Length > 1)
            {
                var cookingTime = timing[1].TextContent
                .Replace("Готвене", string.Empty)
                .Replace(" мин.", string.Empty);
                recipe.CookingTime = TimeSpan.ParseExact(cookingTime, "mm", CultureInfo.InvariantCulture);
                // Console.WriteLine(cookingTime);
            }

            var portionCount = document
                .QuerySelectorAll(".mbox > .feat > span")
                .LastOrDefault().TextContent;
            recipe.PortionsCount = int.Parse(portionCount);
            // Console.WriteLine(portionCount.TextContent);

            var imageUrl = document
                .QuerySelector("#newsGal > div.image > img")
                .GetAttribute("src");
            // Console.WriteLine(imageUrl);

            var ingredients = document.QuerySelectorAll(".products li");
            foreach (var ingredient in ingredients)
            {
                var ingredientInfo = ingredient.TextContent.Split("-");

                if (ingredientInfo.Length < 2)
                {
                    continue;
                }

                var ingredientName = ingredientInfo[0].Trim();
                var ingredientQuantity = ingredientInfo[1].Trim();

                recipe.IngredientsName.Add(ingredientName);
                recipe.IngredientsQuantity.Add(ingredientQuantity);

                // Console.WriteLine(ingredient.TextContent);
            }

            return recipe;

            // Console.WriteLine($"{id} found.");
        }
    }
}
