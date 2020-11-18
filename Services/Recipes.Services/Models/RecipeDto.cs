﻿namespace Recipes.Services.Models
{
    using System;
    using System.Collections.Generic;

    public class RecipeDto
    {
        public RecipeDto()
        {
            this.Ingredients = new List<string>();
        }

        public string CategoryName { get; set; }

        public string RecipeName { get; set; }

        public string Instructions { get; set; }

        public TimeSpan PreparationTime { get; set; }

        public TimeSpan CookingTime { get; set; }

        public int PortionsCount { get; set; }

        public string OriginalUrl { get; set; }

        public string Extension { get; set; }

        public List<string> Ingredients { get; set; }
    }
}
