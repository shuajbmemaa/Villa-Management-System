using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;
using Villa.ViewModels;

namespace Villa.Controllers
{
    [Authorize(Roles =Const.Role_Admin)] //akses ka vec admini 
    public class AmenityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AmenityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;           
        }
        public IActionResult Index()
        {
            var amenities = _unitOfWork.Amenity.GetAll(includeProperties: "Hotel");
            return View(amenities);
        }

        public IActionResult Create()
        {
            AmenityVM amenityVM = new()
            {
                HotelList= _unitOfWork.Hotel.GetAll().Select(u=> new SelectListItem
                {
                    Text = u.Name,
                    Value=u.Id.ToString()
                })
            };
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Create(AmenityVM obj)
        {
            if(ModelState.IsValid)
            {
            _unitOfWork.Amenity.Add(obj.Amenity);
            _unitOfWork.Save();
            TempData["success"] = "Amenity has been created successfully.";
            return RedirectToAction("Index");
            }
            obj.HotelList = _unitOfWork.Hotel.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        public IActionResult Update(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                HotelList = _unitOfWork.Hotel.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error","Home");
            }
            return View(amenityVM) ;
        }

        [HttpPost]
        public IActionResult Update(AmenityVM amenityVM)
        {
            // ModelState.Remove("Hotel");
            if (ModelState.IsValid)
            {
                _unitOfWork.Amenity.Update(amenityVM.Amenity);
                _unitOfWork.Save();
                TempData["success"] = "Amenity has been updated successfully.";
                return RedirectToAction("Index");
            }

            amenityVM.HotelList = _unitOfWork.Hotel.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(amenityVM);
        }

        public IActionResult Delete(int amenityId)
        {
            AmenityVM amenityVM = new()
            {
                HotelList = _unitOfWork.Hotel.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Amenity = _unitOfWork.Amenity.Get(u => u.Id == amenityId)
            };
            if (amenityVM.Amenity == null)
            {
                return RedirectToAction("Error", "Home");
            }
            return View(amenityVM);
        }

        [HttpPost]
        public IActionResult Delete(AmenityVM amenityVM)
        {
            if (amenityVM == null || amenityVM.Amenity == null)
            {
                TempData["error"] = "Amenity object is null.";
                return View(amenityVM);
            }

            Amenity? objfromDB = _unitOfWork.Amenity.Get(u => u.Id == amenityVM.Amenity.Id);
            if (objfromDB != null)
            {
                _unitOfWork.Amenity.Remove(objfromDB);
                _unitOfWork.Save();
                TempData["success"] = "Amenity was deleted successfully.";
                return RedirectToAction("Index", "Amenity");
            }
            TempData["error"] = "Amenity couldn't be deleted.";
            return View(amenityVM);
        }
    }
}
