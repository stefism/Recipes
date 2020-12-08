using AutoMapper;
using Recipes.Data.Models;
using Recipes.Services.Mapping;

namespace Recipes.Web.ViewModels.Recipes
{
    public class EditRecipeInputModel : BaseRecipeInputModel, IMapFrom<Recipe>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public void CreateMappings(IProfileExpression cfg)
        {
            cfg.CreateMap<Recipe, EditRecipeInputModel>()
                .ForMember(x => x.CookingTime, opt =>
                opt.MapFrom(x => (int)x.CookingTime.TotalMinutes))
                .ForMember(x => x.PreparationTime, opt =>
                opt.MapFrom(x => (int)x.PreparationTime.TotalMinutes));
        }
    }
}
