namespace Recipes.Web.ViewModels.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using global::Recipes.Common.Constants;

    public class CreateRecipeInputModel
    {
        [Required(ErrorMessage = BgUIMessages.ThisFieldIsRequired)]
        [MinLength(4, ErrorMessage = BgUIMessages.RecipeNameMustBe4Characters)]
        [Display(Name = BgUIMessages.RecipeName)]
        public string Name { get; set; }

        [Required(ErrorMessage = BgUIMessages.ThisFieldIsRequired)]
        [MinLength(100, ErrorMessage = BgUIMessages.InstructionMustBe100Characters)]
        [Display(Name = BgUIMessages.CookInstructions)]
        public string Instructions { get; set; }

        [Range(0, 24 * 60, ErrorMessage = BgUIMessages.PrepTimeMustBe2Days)]
        [Display(Name ="Време за подготовка в минути:")]
        public int PreparationTime { get; set; }

        [Range(0, 24 * 60, ErrorMessage = "Времето за готвене трябва да е максимално два дни, изчислено в минути.")]
        [Display(Name ="Време за готвене в минути:")]
        public int CookingTime { get; set; }

        [Range(1, 100, ErrorMessage = "Броят порции трябва да бъде в интервала от 1 до 100.")]
        [Display(Name = "Брой порции:")]
        public int PortionCount { get; set; }

        [Display(Name = "Изберете категория:")]
        public int CategoryId { get; set; }

        public IEnumerable<RecipeIngredientInputModel> Ingredients { get; set; }

        public IEnumerable<KeyValuePair<string, string>> CategoriesItems { get; set; }
    }
}
