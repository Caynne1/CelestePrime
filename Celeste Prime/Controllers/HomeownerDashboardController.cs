using Microsoft.AspNetCore.Mvc;

namespace CelestePrime.Controllers
{
    public class HomeownerDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
