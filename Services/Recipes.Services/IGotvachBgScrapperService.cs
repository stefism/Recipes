namespace Recipes.Services
{
    using System.Threading.Tasks;

    public interface IGotvachBgScrapperService
    {
        Task PopulateDbWithRecipesAsync(int recipesCount);
    }
}
