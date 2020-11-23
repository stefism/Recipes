namespace Recipes.Services
{
    using System;
    using System.Collections.Concurrent;
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
        private readonly IDeletableEntityRepository<Recipe> recipeRepository;
        private readonly IRepository<RecipeIngredient> recipeIngredientRepository;
        private readonly IRepository<Image> imageRepository;

        public GotvachBgScrapperService(IDeletableEntityRepository<Category> categoryRepository, IDeletableEntityRepository<Ingredient> ingredientRepository, IDeletableEntityRepository<Recipe> recipeRepository, IRepository<RecipeIngredient> recipeIngredientRepository, IRepository<Image> imageRepository)
        {
            this.config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(this.config);

            this.categoryRepository = categoryRepository;
            this.ingredientRepository = ingredientRepository;
            this.recipeRepository = recipeRepository;
            this.recipeIngredientRepository = recipeIngredientRepository;
            this.imageRepository = imageRepository;
        }

        public async Task PopulateDbWithRecipesAsync(int recipesCount)
        {
            var concurentBag = new ConcurrentBag<RecipeDto>();

            Parallel.For(1, recipesCount, (i) =>
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

                bool recipeExist = this.recipeRepository.AllAsNoTracking()
                    .Any(r => r.Name == recipe.RecipeName);

                if (recipeExist)
                {
                    continue;
                }

                var newRecipe = new Recipe
                {
                    Name = recipe.RecipeName,
                    Instructions = recipe.Instructions,
                    PreparationTime = recipe.PreparationTime,
                    CookingTime = recipe.CookingTime,
                    PortionCount = recipe.PortionsCount,
                    OriginalUrl = recipe.OriginalUrl,
                    CategoryId = categoryId,
                };

                await this.recipeRepository.AddAsync(newRecipe);
                await this.recipeRepository.SaveChangesAsync();

                foreach (var item in recipe.Ingredients)
                {
                    var ingr = item.Split(" - ", 2);
                    if (ingr.Length < 2)
                    {
                        continue;
                    }

                    var ingredientId = await this.GetOrCreateIngredientAsync(ingr[0].Trim());
                    var qty = ingr[1].Trim();

                    var recipeIngredient = new RecipeIngredient
                    {
                        IngredientId = ingredientId,
                        RecipeId = newRecipe.Id,
                        Quantity = qty,
                    };

                    await this.recipeIngredientRepository.AddAsync(recipeIngredient);
                    await this.recipeIngredientRepository.SaveChangesAsync();
                }

                var image = new Image
                {
                    Extension = recipe.OriginalUrl,
                    RecipeId = newRecipe.Id,
                };

                await this.imageRepository.AddAsync(image);
                await this.imageRepository.SaveChangesAsync();
            }
        }

        private async Task<int> GetOrCreateIngredientAsync(string name)
        {
            var ingredient = this.ingredientRepository.AllAsNoTracking()
                .FirstOrDefault(x => x.Name == name);

            if (ingredient == null)
            {
                ingredient = new Ingredient
                {
                    Name = name,
                };

                await this.ingredientRepository.AddAsync(ingredient);
                await this.ingredientRepository.SaveChangesAsync();
            }

            return ingredient.Id;
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
                await this.categoryRepository.SaveChangesAsync();
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

                recipe.PreparationTime = TimeSpan.FromMinutes(int.Parse(prepTime));
                // Console.WriteLine(prepTime);
            }

            if (timing.Length > 1)
            {
                var cookingTime = timing[1].TextContent
                .Replace("Готвене", string.Empty)
                .Replace(" мин.", string.Empty);
                recipe.CookingTime = TimeSpan.FromMinutes(int.Parse(cookingTime));
                // Console.WriteLine(cookingTime);
            }

            var portionCount = document
                .QuerySelectorAll(".mbox > .feat > span")
                .LastOrDefault().TextContent;
            recipe.PortionsCount = int.Parse(portionCount);
            // Console.WriteLine(portionCount.TextContent);

            recipe.OriginalUrl = document
                .QuerySelector("#newsGal > div.image > img")
                .GetAttribute("src");
            // Console.WriteLine(imageUrl);

            var ingredients = document.QuerySelectorAll(".products li");

            foreach (var ingredient in ingredients)
            {
                recipe.Ingredients.Add(ingredient.TextContent);
            }

            return recipe;
        }
    }
}
