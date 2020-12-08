using Recipes.Web.ViewModels.Recipes;
using System.Collections.Generic;

namespace Recipes.Web.ViewModels.SearchRecipes
{
    public class ListViewModel
    {
        public IEnumerable<RecipeInListViewModel> Recipes { get; set; }
    }
}
