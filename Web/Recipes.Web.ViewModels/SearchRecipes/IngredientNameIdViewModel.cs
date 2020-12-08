using Recipes.Data.Models;
using Recipes.Services.Mapping;

namespace Recipes.Web.ViewModels.SearchRecipes
{
    public class IngredientNameIdViewModel : IMapFrom<Ingredient>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
