using Microsoft.AspNetCore.Mvc;

namespace Villa.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
