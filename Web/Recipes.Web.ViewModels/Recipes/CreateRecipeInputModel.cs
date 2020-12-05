namespace Recipes.Web.ViewModels.Recipes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using global::Recipes.Common.Constants;
    using Microsoft.AspNetCore.Http;

    public class CreateRecipeInputModel : BaseRecipeInputModel
    {
        // [Range(1, 10, ErrorMessage = BgCreateRecipeMessages.ImagesBetween0And10)]
        [Display(Name = BgCreateRecipeMessages.ChooseImages)]
        public IEnumerable<IFormFile> Images { get; set; }

        public IEnumerable<RecipeIngredientInputModel> Ingredients { get; set; }
    }
}
