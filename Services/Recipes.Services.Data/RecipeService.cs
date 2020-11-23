namespace Recipes.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Recipes.Data.Common.Repositories;
    using Recipes.Data.Models;
    using Recipes.Services.Mapping;
    using Recipes.Web.ViewModels.Recipes;

    public class RecipeService : IRecipeService
    {
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public RecipeService(IDeletableEntityRepository<Recipe> recipesRepository, IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.recipesRepository = recipesRepository;
            this.ingredientsRepository = ingredientsRepository;
        }

        public async Task CreateAsync(CreateRecipeInputModel input, string userId)
        {
            var recipe = new Recipe
            {
                Name = input.Name,
                Instructions = input.Instructions,
                CategoryId = input.CategoryId,
                CookingTime = TimeSpan.FromMinutes(input.CookingTime),
                PreparationTime = TimeSpan.FromMinutes(input.PreparationTime),
                PortionCount = input.PortionCount,
                AddedByUserId = userId,
            };

            foreach (var inputIngredient in input.Ingredients)
            {
                var ingredient = this.ingredientsRepository.All().FirstOrDefault(x => x.Name == input.Name);

                if (ingredient == null)
                {
                    ingredient = new Ingredient
                    {
                        Name = inputIngredient.IngredientName,
                    };
                }

                recipe.Ingredients.Add(new RecipeIngredient
                {
                    Ingredient = ingredient,
                    Quantity = inputIngredient.Quantity,
                });
            }

            await this.recipesRepository.AddAsync(recipe);
            await this.recipesRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 12)
        {
            var recipes = this.recipesRepository.AllAsNoTracking()
                .OrderByDescending(r => r.Id)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .To<T>()
                //.Select(r => new RecipeInListViewModel
                //{
                //    Id = r.Id,
                //    Name = r.Name,
                //    CategoryName = r.Category.Name,
                //    CategoryId = r.CategoryId,
                //    ImageUrl =
                //    r.Images.FirstOrDefault().RemoteImageUrl != null
                //    ? r.Images.FirstOrDefault().RemoteImageUrl
                //    : "/images/recipes" + r.Images.FirstOrDefault().Id + "." + r.Images.FirstOrDefault().Extension,
                //}) //Ако ползваме ауто мапера, всичкото това се икономисва.
                .ToList();

            return recipes;

            // Skip и Take работят само ако имаме сортиране ( в случая - OrderByDescending).
        }

        public int GetCount()
        {
            return this.recipesRepository.All().Count();
        }
    }
}
