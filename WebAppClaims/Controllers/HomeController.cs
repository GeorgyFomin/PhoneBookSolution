using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebAppClaims.Models;

namespace WebAppClaims.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Authorize(Policy = "AgeLimit")]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult About()
        {
            return Content("For all ages");
        }
        //[Authorize(Policy = "OnlyForLondon")]
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[Authorize(Policy = "OnlyForMicrosoft")]
        //public IActionResult About()
        //{
        //    return Content("Only for Microsoft employees");
        //}
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}