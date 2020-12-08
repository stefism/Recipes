namespace Recipes.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using Recipes.Data.Common.Repositories;
    using Recipes.Data.Models;
    using Recipes.Services.Mapping;

    public class IngredientsService : IIngredientsService
    {
        private readonly IDeletableEntityRepository<Ingredient> ingredientsRepository;

        public IngredientsService(IDeletableEntityRepository<Ingredient> ingredientsRepository)
        {
            this.ingredientsRepository = ingredientsRepository;
        }

        public IEnumerable<T> GetAll<T>()
        {
            return this.ingredientsRepository.All()
                .OrderByDescending(x => x.Recipes.Count)
                .ThenBy(x => x.Name)
                .To<T>().ToList();
        }
    }
}
