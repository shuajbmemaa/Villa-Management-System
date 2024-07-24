using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Villa.Application.Common.Interfaces;
using Villa.Models;
using Villa.ViewModels;

namespace Villa.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM homeVM = new()
            {
                HotelList = _unitOfWork.Hotel.GetAll(includeProperties: "HotelAmenity"),
                Nights=1,
                CheckInDate=DateOnly.FromDateTime(DateTime.Now),
            };
            return View(homeVM);
        }

        [HttpPost]
        public IActionResult Index(HomeVM homeVM)
        {
            homeVM.HotelList = _unitOfWork.Hotel.GetAll(includeProperties: "HotelAmenity");
            return View(homeVM);
        }

        public IActionResult GetHotelsByDate(int nights,DateOnly checkInDate)
        {
            var hotelList = _unitOfWork.Hotel.GetAll(includeProperties: "HoteliList").ToList();
            foreach (var hotel in hotelList)
            {
                if (hotel.Id % 2 == 0)
                {
                    hotel.IsAvailable = false;
                }
            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                HotelList = hotelList,
                Nights = nights
            };

            return View(homeVM);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
