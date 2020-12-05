using Recipes.Data.Models;
using Recipes.Services.Mapping;

namespace Recipes.Web.ViewModels.Recipes
{
    public class EditRecipeInputModel : BaseRecipeInputModel, IMapFrom<Recipe>
    {
        public int Id { get; set; }
    }
}
