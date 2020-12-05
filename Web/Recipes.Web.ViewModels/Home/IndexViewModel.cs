namespace Recipes.Web.ViewModels.Home
{
    using System.Collections.Generic;

    public class IndexViewModel
    {
        public IEnumerable<IndexPageRecipeViewModel> RandomRecipes { get; set; }

        public int RecepiesCount { get; set; }

        public int CategoriesCount { get; set; }

        public int IngredientsCount { get; set; }

        public int ImagesCount { get; set; }
    }
}
