using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
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
        public IActionResult GetHotelsByDate(int nights,DateOnly checkInDate)
        {
            var hotelList = _unitOfWork.Hotel.GetAll(includeProperties: "HotelAmenity").ToList();
            var hotelNumberList = _unitOfWork.HotelNumber.GetAll().ToList();
            var bookedHotels = _unitOfWork.Booking.GetAll(u => u.Status == Const.StatusApproved || u.Status == Const.StatusCheckedIn).ToList();

            foreach (var hotel in hotelList)
            {
                int roomAvailable=Const.HotelRoomsAvailable(hotel.Id, hotelNumberList,checkInDate,nights,bookedHotels);

                hotel.IsAvailable= roomAvailable > 0 ? true : false;
            }
            HomeVM homeVM = new()
            {
                CheckInDate = checkInDate,
                HotelList = hotelList,
                Nights = nights
            };

            return PartialView("_HotelList",homeVM);//kur dojna me bo reload ni pjes te caktume te faqes e perdorum PartialView 
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
