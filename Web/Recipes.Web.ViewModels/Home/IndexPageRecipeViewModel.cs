using AutoMapper;
using Recipes.Data.Models;
using Recipes.Services.Mapping;
using System.Linq;

namespace Recipes.Web.ViewModels.Home
{
    public class IndexPageRecipeViewModel :
        IMapFrom<Recipe>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Recipe, IndexPageRecipeViewModel>()
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
