namespace Recipes.Services.Data
{
    using System.Threading.Tasks;

    using Recipes.Web.ViewModels.Recipes;

    public interface IRecipeService
    {
        Task CreateAsync(CreateRecipeInputModel input);
    }
}
