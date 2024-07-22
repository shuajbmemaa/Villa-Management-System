using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Controllers
{
    [Authorize] //nese e len veq authorize pa u bo login nuk mundesh me pas akses te hotel controlleri
    public class HotelController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HotelController(IUnitOfWork unitOfWork,IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;      
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var hotels = _unitOfWork.Hotel.GetAll();
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
                if(hotel.Image != null)
                {
                    string fileName=Guid.NewGuid().ToString()+ Path.GetExtension(hotel.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\Hotel_Images");

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        hotel.Image.CopyTo(fileStream);

                    hotel.ImageUrl = @"\Images\Hotel_Images\" + fileName;
                }
                else
                {
                    hotel.ImageUrl = "https://placehold.co/600x400";
                }
            _unitOfWork.Hotel.Add(hotel);
            _unitOfWork.Save();
            TempData["success"] = "Hotel was created successfully.";
            return RedirectToAction("Index","Hotel");
            }
            return View(hotel);
        }

        public IActionResult Update(int hotelId)//qysh e ke lan n Index.cshtml te asp-action duhet me kan edhe emri i metodes , poashtu cka pranon si parameter duhet me kan se cka i ke caktu ti te asp-route-...
        {
            Hotel? hotel=_unitOfWork.Hotel.Get(u=>u.Id == hotelId);
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
                if (hotel.Image != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(hotel.Image.FileName);
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, @"Images\Hotel_Images");

                    if (!string.IsNullOrEmpty(hotel.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, hotel.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(imagePath, fileName), FileMode.Create))
                        hotel.Image.CopyTo(fileStream);

                    hotel.ImageUrl = @"\Images\Hotel_Images\" + fileName;
                }

                _unitOfWork.Hotel.Update(hotel);
                _unitOfWork.Save();
                TempData["success"] = "Hotel was updated successfully.";
                return RedirectToAction("Index", "Hotel");
            }
            return View(hotel);
        }

        public IActionResult Delete(int hotelId)//qysh e ke lan n Index.cshtml te asp-action duhet me kan edhe emri i metodes , poashtu cka pranon si parameter duhet me kan se cka i ke caktu ti te asp-route-...
        {
            Hotel? hotel = _unitOfWork.Hotel.Get(h => h.Id == hotelId);
            if (hotel == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(hotel);
        }

        [HttpPost]
        public IActionResult Delete(Hotel hotel)
        {
            Hotel? objfromDB= _unitOfWork.Hotel.Get(h=>h.Id == hotel.Id);
            if (objfromDB is not null)
            {

                if (!string.IsNullOrEmpty(objfromDB.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, objfromDB.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _unitOfWork.Hotel.Remove(objfromDB);
                _unitOfWork.Save();
                TempData["success"] = "Hotel was deleted successfully.";
                return RedirectToAction("Index", "Hotel");
            }
            TempData["error"] = "Hotel couldn't be deleted.";
            return View(hotel);
        }
    }
}
