using Recipes.Data.Models;
using Recipes.Services.Mapping;

namespace Recipes.Web.ViewModels.Recipes
{
    public class IngredientsViewModel : IMapFrom<RecipeIngredient>
    {
        public string IngredientName { get; set; }

        public string Quantity { get; set; }
    }
}
