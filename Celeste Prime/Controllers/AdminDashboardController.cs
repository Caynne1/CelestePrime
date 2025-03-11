using Microsoft.AspNetCore.Mvc;

namespace CelestePrime.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
