namespace Recipes.Web.ViewModels.Recipes
{
    using System;
    using System.Collections.Generic;

    public class RecipesListViewModel : PagingViewModel
        // На всякъде, където ще ползваме паршал вю, наследяваме вю модела на паршал вюто, към основния ни вю модел, който е за съответната страница.
    {
        public IEnumerable<RecipeInListViewModel> Recipes { get; set; }
    }
}
