namespace Recipes.Services.Data.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Moq;
    using Recipes.Data.Common.Repositories;
    using Recipes.Data.Models;
    using Xunit;

    public class VotesServiceTests
    {
        [Fact]
        public async Task WhenUserVotes2TimesOnly1VoteShouldBeCounted()
        {
            var votesList = new List<Vote>();
            // Това все едно в момента ни е таблицата Votes от базата данни.

            var mockRepo = new Mock<IRepository<Vote>>();
            mockRepo.Setup(method => method.All())
                .Returns(votesList.AsQueryable());
            // Когато някой ти извика медода .All(), ти му върни votesList.AsQueryable().

            mockRepo.Setup(m => m.AddAsync(It.IsAny<Vote>()))
                .Callback((Vote vote) => votesList.Add(vote));
            // Когато имаме параметри на метода, в случая AddAsync иска да му се подаде Vote, със It.IsAny му казваме, ползвай това в метода, независимо как е подадено.

            // var repo = new FakeVotesRepository();
            // var service = new VotesService(repo);

            var service = new VotesService(mockRepo.Object);

            await service.SetVoteAsync(1, "1", 1);
            await service.SetVoteAsync(1, "1", 5);
            await service.SetVoteAsync(1, "1", 5);
            await service.SetVoteAsync(1, "1", 5);
            await service.SetVoteAsync(1, "1", 5);

            // Assert.Equal(1, repo.All().Count());
            // Assert.Equal(5, repo.All().First().Value);

            Assert.Equal(1, votesList.Count());
            Assert.Equal(5, votesList.First().Value);
        }

        [Fact]
        public async Task When2UsersVoteForTheSameRecipeTheAverageVoteShouldBeCorrect()
        {
            var votesList = new List<Vote>();

            var mockRepo = new Mock<IRepository<Vote>>();
            mockRepo.Setup(method => method.All())
                .Returns(votesList.AsQueryable());

            mockRepo.Setup(m => m.AddAsync(It.IsAny<Vote>()))
                .Callback((Vote vote) => votesList.Add(vote));

            var service = new VotesService(mockRepo.Object);

            await service.SetVoteAsync(2, "Niki", 5);
            await service.SetVoteAsync(2, "Pesho", 1);
            await service.SetVoteAsync(2, "Niki", 2);

            // mockRepo.Verify(m => m.All(), Times.Exactly(3));
            // Можем да проверим колко пъти е извикан даден метод. В случая, дали метода All() е извикан точно три пъти.

            mockRepo.Verify(m => m.AddAsync(It.IsAny<Vote>()), Times.Exactly(2));

            Assert.Equal(2, votesList.Count());
            Assert.Equal(1.5, service.GetAverageVotes(2));
        }
    }

    public class FakeVotesRepository : IRepository<Vote>
    {
        private List<Vote> list = new List<Vote>();

        public Task AddAsync(Vote entity)
        {
            this.list.Add(entity);
            return Task.CompletedTask;
        }

        public IQueryable<Vote> All()
        {
            return this.list.AsQueryable();
        }

        public IQueryable<Vote> AllAsNoTracking()
        {
            return this.list.AsQueryable();
        }

        public void Delete(Vote entity)
        {
        }

        public void Dispose()
        {
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(0);
        }

        public void Update(Vote entity)
        {
        }
    }
}
