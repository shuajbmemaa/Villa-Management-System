using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Villa.Application.Common.Interfaces;
using Villa.Application.Common.Utility;
using Villa.Domain.Entities;

namespace Villa.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult FinalizeBooking(int hotelId,DateOnly checkInDate,int nights)
        {
            var claimsIdentity=(ClaimsIdentity)User.Identity;
            var userId=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;//useri i kyct momentalisht

            User user =_unitOfWork.User.Get(u=>u.Id==userId);

            Booking booking = new()
            {
                HotelId = hotelId,
                Hotel=_unitOfWork.Hotel.Get(u=>u.Id==hotelId,includeProperties:"HotelAmenity"),//HotelAmenity te modeli Hoteli , IEnumerable
                CheckInDate=checkInDate,
                Nights=nights,
                CheckOutDate=checkInDate.AddDays(nights),
                UserId=userId,
                Phone=user.PhoneNumber,
                Email=user.Email,
                Name=user.Name
            };
            booking.Total = booking.Hotel.Price * nights;
            return View(booking);
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var hotel = _unitOfWork.Hotel.Get(y => y.Id == booking.HotelId);
            booking.Total =  hotel.Price * booking.Nights;
            booking.Status=Const.StatusPending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();
            return RedirectToAction(nameof(BookingConfirmation),new { bookingId = booking.Id}); // new bookingId duhet me kan e njejt si parametri te metoda BookingConfirmation
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            return View(bookingId);
        }
    }
}
