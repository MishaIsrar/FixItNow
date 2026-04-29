using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FixItNow.Models;
using Microsoft.AspNetCore.Authorization;
using FixItNow.Data;

namespace FixItNow.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext c;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext d)

        {
            _logger = logger;
            c = d;
        }

        public IActionResult Index()
        {
            var list=c.Reviews.ToList();
            return View(list);
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult FAQS()
        {
            return View();
        }
        public IActionResult ContactUs()
        {
            return View();
        }
        public IActionResult Catalogue()
        {
            return View();
        }
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
