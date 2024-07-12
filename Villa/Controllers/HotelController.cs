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
            TempData["success"] = "Hotel was created successfully.";
            return RedirectToAction("Index","Hotel");
            }
            return View(hotel);
        }

        public IActionResult Update(int hotelId)//qysh e ke lan n Index.cshtml te asp-action duhet me kan edhe emri i metodes , poashtu cka pranon si parameter duhet me kan se cka i ke caktu ti te asp-route-...
        {
            Hotel? hotel=_db.Hotels.FirstOrDefault(h => h.Id == hotelId);
            if(hotel == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(hotel) ;
        }

        [HttpPost]
        public IActionResult Update(Hotel hotel)
        {
            if (ModelState.IsValid && hotel.Id >0)
            {
                _db.Hotels.Update(hotel);
                _db.SaveChanges();
                TempData["success"] = "Hotel was updated successfully.";
                return RedirectToAction("Index", "Hotel");
            }
            return View(hotel);
        }

        public IActionResult Delete(int hotelId)//qysh e ke lan n Index.cshtml te asp-action duhet me kan edhe emri i metodes , poashtu cka pranon si parameter duhet me kan se cka i ke caktu ti te asp-route-...
        {
            Hotel? hotel = _db.Hotels.FirstOrDefault(h => h.Id == hotelId);
            if (hotel == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(hotel);
        }

        [HttpPost]
        public IActionResult Delete(Hotel hotel)
        {
            Hotel? objfromDB=_db.Hotels.FirstOrDefault(h=>h.Id == hotel.Id);
            if (objfromDB is not null)
            {
                _db.Hotels.Remove(objfromDB);
                _db.SaveChanges();
                TempData["success"] = "Hotel was deleted successfully.";
                return RedirectToAction("Index", "Hotel");
            }
            TempData["error"] = "Hotel couldn't be deleted.";
            return View(hotel);
        }
    }
}
