using AutoMapper;
using Recipes.Data.Models;
using Recipes.Services.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Recipes.Web.ViewModels.Recipes
{
    public class SingleRecipeViewModel
        : IMapFrom<Recipe>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CategoryName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string AddedByUserUserName { get; set; } // Това се кръщава така за да може да сработи ауто мапера.

        public string ImageUrl { get; set; }

        public string Instructions { get; set; }

        public TimeSpan PreparationTime { get; set; }

        public TimeSpan CookingTime { get; set; }

        public int PortionCount { get; set; }

        public int CategoryRecipesCount { get; set; }

        public string OriginalUrl { get; set; }

        public double VotesAverageValue { get; set; }

        public IEnumerable<IngredientsViewModel> Ingredients { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Recipe, SingleRecipeViewModel>()
                .ForMember(x => x.VotesAverageValue, opt =>
                opt.MapFrom(x => x.Votes.Average(v => v.Value)))

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
