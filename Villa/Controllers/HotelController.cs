using Microsoft.AspNetCore.Mvc;
using Villa.Infrastructure.Data;

namespace Villa.Controllers
{
    public class HotelController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HotelController(ApplicationDbContext db)
        {
            _db = db;           
        }
        public IActionResult Index()
        {
            var hotels = _db.Hotels.ToList();
            return View(hotels);
        }
    }
}
