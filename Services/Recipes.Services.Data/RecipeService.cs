namespace Recipes.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Recipes.Data.Common.Repositories;
    using Recipes.Data.Models;
    using Recipes.Services.Mapping;
    using Recipes.Web.ViewModels.Recipes;

    public class RecipeService : IRecipeService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "gif" };
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;
        private readonly IWebHostEnvironment environment; // Това поле ни трябва за работа с файлове в сървиса. За да работи трябва да направим няколко неща в Recipes.Services.Data.csproj:
        // 1. Променяме най-горе да стане <Project Sdk="Microsoft.NET.Sdk.Web">.
        // 2. Променяме да стане <TargetFramework>net5.0</TargetFramework>.
        // Инсталираме 2 допълнителни пакета:
        // а) Microsoft.AspNetCore.Hosting.Abstractions
        // б) Microsoft.AspNetCore.Http.Features
        // 3. Трябва да си направим Program class със Main метод, щото иначе се оплаква, защото е web.
        // Това обаче се оказа, че не се прави тука и може да се напарви по-умно!

        public RecipeService(IDeletableEntityRepository<Recipe> recipesRepository, IDeletableEntityRepository<Ingredient> ingredientsRepository, IWebHostEnvironment environment)
        {
            this.recipesRepository = recipesRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.environment = environment;
        }

        public async Task CreateAsync(CreateRecipeInputModel input, string userId, string imagePath)
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

            // /wwwroot/images/recipes/{id}.{ext}
            Directory.CreateDirectory($"{imagePath}/recipes/");
            foreach (var image in input.Images)
            {
                var extension = Path
                    .GetExtension(image.FileName).TrimStart('.');
                if (!this.allowedExtensions.Contains(extension)) //Леко различно при Ники.
                {
                    throw new Exception($"Invalid image extension {extension}");
                }

                var dbImage = new Image
                {
                    AddedByUserId = userId,
                    Extension = extension,
                };
                recipe.Images.Add(dbImage);

                var physicalPath = $"{imagePath}/recipes/{dbImage.Id}.{extension}"; // Ето така се правело!

                using Stream fs = new FileStream(physicalPath, FileMode.Create);
                await image.CopyToAsync(fs);

                // var physicalPath_pr = $"{this.environment.ContentRootPath}/images/recipes/{dbImage.Id}.{extension}";
                // this.environment.ContentRootPath - взима пътя на wwwroot в случая или на директорията със споделените файлове. Това е само е така.
            }

            await this.recipesRepository.AddAsync(recipe);
            await this.recipesRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var recipe = this.recipesRepository.All().FirstOrDefault(x => x.Id == id);
            this.recipesRepository.Delete(recipe);
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

        public T GetById<T>(int id)
        {
            var recipe = this.recipesRepository.AllAsNoTracking()
                .Where(x => x.Id == id).To<T>().FirstOrDefault();

            return recipe;
        }

        public IEnumerable<T> GetByIngredients<T>(IEnumerable<int> ingredientsIds)
        {
            var query = this.recipesRepository.All().AsQueryable();
            // При този метод, заявката все още не е изпратена до базата и това ни позволява да натрупваме няколко условия едно след друго към заявката и накрая да я пратим към базата.
            foreach (var ingredientId in ingredientsIds)
            {
                query = query.Where(r => r.Ingredients.Any(i => i.IngredientId == ingredientId)); // Трупаме where клаузи към заявката (query).
            }

            return query.To<T>().ToList();
        }

        public int GetCount()
        {
            return this.recipesRepository.All().Count();
        }

        public IEnumerable<T> GetRandom<T>(int count)
        {
            return this.recipesRepository.All()
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .To<T>().ToList();
            // Guid.NewGuid() - специаленс случай в EF, където му се казва да сортира по случаен принцип.
            // To<T> - ауто мапва от Recipe към IndexPageRecipeViewModel.
        }

        public async Task UpdateAsync(int id, EditRecipeInputModel input)
        {
            var recipe = this.recipesRepository.All().FirstOrDefault(x => x.Id == id);

            recipe.Name = input.Name;
            recipe.Instructions = input.Instructions;
            recipe.CookingTime = TimeSpan.FromMinutes(input.CookingTime);
            recipe.PreparationTime = TimeSpan.FromMinutes(input.PreparationTime);
            recipe.PortionCount = input.PortionCount;
            recipe.CategoryId = input.CategoryId;

            await this.recipesRepository.SaveChangesAsync();
        }
    }
}
