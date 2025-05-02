using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SistemaLeilao_web.Models; // Assuming ErrorViewModel might be used

namespace SistemaLeilao_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // TODO: Fetch auction data from API later
            return View();
        }

        // Placeholder for Privacy page if needed
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

