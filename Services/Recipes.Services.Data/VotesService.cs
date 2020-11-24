using Recipes.Data.Common.Repositories;
using Recipes.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recipes.Services.Data
{
    public class VotesService : IVotesService
    {
        private readonly IRepository<Vote> votesRepo;

        public VotesService(IRepository<Vote> votesRepo)
        {
            this.votesRepo = votesRepo;
        }

        public double GetAverageVotes(int recipeId)
        {
            return this.votesRepo.All().Where(x => x.RecipeId == recipeId)
                .Average(x => x.Value);
        }

        public async Task SetVoteAsync(int recipeId, string userId, byte value)
        {
            var vote = this.votesRepo.All().FirstOrDefault(v => v.RecipeId == recipeId && v.UserId == userId);

            if (vote == null)
            {
                vote = new Vote
                {
                    RecipeId = recipeId,
                    UserId = userId,
                };

                await this.votesRepo.AddAsync(vote);
            }

            vote.Value = value;
            await this.votesRepo.SaveChangesAsync();
        }
    }
}
