using Microsoft.AspNetCore.Mvc;
using Villa.Domain.Entities;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Hotel hotel)
        {
            if(hotel.Name == hotel.Description)
            {
                ModelState.AddModelError("Description", "The Description cannot exactly match the Name");
            }
            if(ModelState.IsValid)
            {
            _db.Hotels.Add(hotel);
            _db.SaveChanges();
            return RedirectToAction("Index","Hotel");
            }
            return View(hotel);
        }
    }
}
