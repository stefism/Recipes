namespace Recipes.Services.Data
{
    using System.Linq;

    using Recipes.Data.Common.Repositories;
    using Recipes.Data.Models;
    using Recipes.Web.ViewModels.Home;

    public class GetCountService : IGetCountService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly IRepository<Image> imagesRepository;
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;
        private readonly IDeletableEntityRepository<Recipe> recipesRepository;

        public GetCountService(IDeletableEntityRepository<Category> categoriesRepository, IRepository<Image> imagesRepository, IDeletableEntityRepository<Ingredient> ingredientsRepository, IDeletableEntityRepository<Recipe> recipesRepository)
        {
            this.categoriesRepository = categoriesRepository;
            this.imagesRepository = imagesRepository;
            this.ingredientsRepository = ingredientsRepository;
            this.recipesRepository = recipesRepository;
        }

        public IndexViewModel GetCounts()
        {
            var viewModel = new IndexViewModel
            {
                CategoriesCount = this.categoriesRepository.All().Count(),
                ImagesCount = this.imagesRepository.All().Count(),
                IngredientsCount = this.ingredientsRepository.All().Count(),
                RecepiesCount = this.recipesRepository.All().Count(),
            };

            return viewModel;
        }
    }
}
