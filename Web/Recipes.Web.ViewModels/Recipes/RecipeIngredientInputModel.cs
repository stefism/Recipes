namespace Recipes.Web.ViewModels.Recipes
{
    using System.ComponentModel.DataAnnotations;

    public class RecipeIngredientInputModel
    {
        [Required(ErrorMessage = "Задължително трябва да попълните това поле.")]
        [Display(Name = "Име на съставката:")]
        [MinLength(3, ErrorMessage = "Името на съставката трябва да бъде с дължина поне три символа.")]
        public string IngredientName { get; set; }

        [Required(ErrorMessage = "Задължително трябва да попълните това поле.")]
        [Display(Name = "Количество на съставката:")]
        [MinLength(3, ErrorMessage = "Количеството на съставката трябва да бъде с дължина поне три символа.")]
        public string Quantity { get; set; }
    }
}
