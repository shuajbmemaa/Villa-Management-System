using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;
using Villa.ViewModels;

namespace Villa.Controllers
{
    public class HotelNumberController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HotelNumberController(ApplicationDbContext db)
        {
            _db = db;           
        }
        public IActionResult Index()
        {
            var hotelNumbers = _db.HotelNumbers.Include(u=>u.Hotel).ToList();
            return View(hotelNumbers);
        }

        public IActionResult Create()
        {
            HotelNumberVM hotelNumberVM = new()
            {
                HotelList= _db.Hotels.ToList().Select(u=> new SelectListItem
                {
                    Text = u.Name,
                    Value=u.Id.ToString()
                })
            };
            return View(hotelNumberVM);
        }

        [HttpPost]
        public IActionResult Create(HotelNumberVM obj)
        {
            bool hotelNumberExists=_db.HotelNumbers.Any(u=>u.Hotel_Nr == obj.HotelNumber.Hotel_Nr);
            //ose
            bool isNumberUnique = _db.HotelNumbers.Where(u => u.Hotel_Nr == obj.HotelNumber.Hotel_Nr).Count() == 0;


           // ModelState.Remove("Hotel");
            if(ModelState.IsValid && !hotelNumberExists)
            {
            _db.HotelNumbers.Add(obj.HotelNumber);
            _db.SaveChanges();
            TempData["success"] = "Hotel Number has been created successfully.";
            return RedirectToAction("Index");
            }
            if (hotelNumberExists)
            {
                TempData["error"] = "The hotel number already exists";
            }
            obj.HotelList = _db.Hotels.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Update(int hotelNumberId)//qysh e ke lan n Index.cshtml te asp-action duhet me kan edhe emri i metodes , poashtu cka pranon si parameter duhet me kan se cka i ke caktu ti te asp-route-...
        {
            HotelNumberVM hotelNumberVM = new()
            {
                HotelList = _db.Hotels.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                HotelNumber = _db.HotelNumbers.FirstOrDefault(u => u.Hotel_Nr == hotelNumberId)
            };
            if (hotelNumberVM.HotelNumber == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(hotelNumberVM) ;
        }

        [HttpPost]
        public IActionResult Update(HotelNumberVM hotelNumberVM)
        {
            // ModelState.Remove("Hotel");
            if (ModelState.IsValid)
            {
                _db.HotelNumbers.Update(hotelNumberVM.HotelNumber);
                _db.SaveChanges();
                TempData["success"] = "Hotel Number has been updated successfully.";
                return RedirectToAction("Index");
            }

            hotelNumberVM.HotelList = _db.Hotels.ToList().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(hotelNumberVM);
            
        }

        public IActionResult Delete(int hotelNumberId)//qysh e ke lan n Index.cshtml te asp-action duhet me kan edhe emri i metodes , poashtu cka pranon si parameter duhet me kan se cka i ke caktu ti te asp-route-...
        {
            HotelNumberVM hotelNumberVM = new()
            {
                HotelList = _db.Hotels.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                HotelNumber = _db.HotelNumbers.FirstOrDefault(u => u.Hotel_Nr == hotelNumberId)
            };
            if (hotelNumberVM.HotelNumber == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(hotelNumberVM);
        }

        [HttpPost]
        public IActionResult Delete(HotelNumberVM hotel)
        {
            HotelNumber? objfromDB=_db.HotelNumbers.FirstOrDefault(h=>h.Hotel_Nr == hotel.HotelNumber.Hotel_Nr);
            if (objfromDB is not null)
            {
                _db.HotelNumbers.Remove(objfromDB);
                _db.SaveChanges();
                TempData["success"] = "Hotel Number was deleted successfully.";
                return RedirectToAction("Index", "HotelNumber");
            }
            TempData["error"] = "Hotel Number couldn't be deleted.";
            return View(hotel);
        }
    }
}
