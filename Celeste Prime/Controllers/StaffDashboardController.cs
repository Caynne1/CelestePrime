using Microsoft.AspNetCore.Mvc;

namespace CelestePrime.Controllers
{
    public class StaffDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
