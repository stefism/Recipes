﻿using Recipes.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recipes.Web.ViewModels.Recipes
{
    public abstract class BaseRecipeInputModel
    {
        [Required(ErrorMessage = BgCreateRecipeMessages.ThisFieldIsRequired)]
        [MinLength(4, ErrorMessage = BgCreateRecipeMessages.RecipeNameMustBe4Characters)]
        [Display(Name = BgCreateRecipeMessages.RecipeName)]
        public string Name { get; set; }

        [Required(ErrorMessage = BgCreateRecipeMessages.ThisFieldIsRequired)]
        [MinLength(100, ErrorMessage = BgCreateRecipeMessages.InstructionMustBe100Characters)]
        [Display(Name = BgCreateRecipeMessages.CookInstructions)]
        public string Instructions { get; set; }

        [Range(0, 24 * 60, ErrorMessage = BgCreateRecipeMessages.PrepTimeMustBe2Days)]
        [Display(Name = BgCreateRecipeMessages.PrepTimeInMinutes)]
        public int PreparationTime { get; set; }

        [Range(0, 24 * 60, ErrorMessage = BgCreateRecipeMessages.CookTimeMustBe2Days)]
        [Display(Name = BgCreateRecipeMessages.CookTimeInMinutes)]
        public int CookingTime { get; set; }

        [Range(1, 100, ErrorMessage = BgCreateRecipeMessages.PortionCountBetween0And100)]
        [Display(Name = BgCreateRecipeMessages.PortionNumber)]
        public int PortionCount { get; set; }

        [Display(Name = BgCreateRecipeMessages.CategoryChoose)]
        public int CategoryId { get; set; }

        public IEnumerable<KeyValuePair<string, string>> CategoriesItems { get; set; }
    }
}
