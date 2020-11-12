namespace Recipes.Web.ViewModels.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CreateRecipeInputModel
    {
        [Required(ErrorMessage = "Задължително трябва да попълните това поле.")]
        [MinLength(4, ErrorMessage = "Името на рецептата трябва да бъде с дължина поне 4 символа.")]
        [Display(Name ="Име на рецептата:")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Задължително трябва да попълните това поле.")]
        [MinLength(100, ErrorMessage = "Инструкцията трябва да е с дължина поне 100 символа.")]
        [Display(Name ="Инструкции за приготвяне:")]
        public string Instructions { get; set; }

        [Range(0, 24 * 60, ErrorMessage = "Времето за готвене трябва да е максимално два дни, изчислено в минути.")]
        [Display(Name ="Време за подготовка в минути:")]
        public int PreparationTime { get; set; }

        [Range(0, 24 * 60, ErrorMessage = "Времето за готвене трябва да е максимално два дни, изчислено в минути.")]
        [Display(Name ="Време за готвене в минути:")]
        public int CookingTime { get; set; }

        [Range(1, 100, ErrorMessage = "Броят порции трябва да бъде в интервала от 1 до 100.")]
        [Display(Name = "Брой порции:")]
        public int PortionCount { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<RecipeIngredientInputModel> Ingredients { get; set; }

        public IEnumerable<KeyValuePair<string, string>> CategoriesItems { get; set; }
    }
}
