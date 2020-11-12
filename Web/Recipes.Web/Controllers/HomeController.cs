namespace Recipes.Web.Controllers
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Mvc;
    using Recipes.Services.Data;
    using Recipes.Web.ViewModels;

    public class HomeController : BaseController
    {
        private readonly IGetCountService countService;

        public HomeController(IGetCountService countService)
        {
            this.countService = countService;
        }

        public IActionResult Index()
        {
            var model = this.countService.GetCounts();

            return this.View(model);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
