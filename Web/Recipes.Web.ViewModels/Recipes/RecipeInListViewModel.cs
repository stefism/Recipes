using AutoMapper;
using Recipes.Data.Models;
using Recipes.Services.Mapping;
using System.Linq;

namespace Recipes.Web.ViewModels.Recipes
{
    public class RecipeInListViewModel : IMapFrom<Recipe>, IHaveCustomMappings

    // IMapFrom<Recipe> и IHaveCustomMappings идват от темплейта на Ники.
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string Name { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Recipe, RecipeInListViewModel>()
                .ForMember(x => x.ImageUrl, opt =>
                opt.MapFrom(x =>
                x.Images.FirstOrDefault().RemoteImageUrl != null
                    ? x.Images.FirstOrDefault().RemoteImageUrl
                    : "/images/recipes/" + x.Images
                    .FirstOrDefault().Id + "." + x.Images
                    .FirstOrDefault().Extension));
        }
    }
}
