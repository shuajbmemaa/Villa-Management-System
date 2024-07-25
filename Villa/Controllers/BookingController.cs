using Microsoft.AspNetCore.Mvc;
using Villa.Application.Common.Interfaces;
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
        public IActionResult FinalizeBooking(int hotelId,DateOnly checkInDate,int nights)
        {
            Booking booking = new()
            {
                HotelId = hotelId,
                Hotel=_unitOfWork.Hotel.Get(u=>u.Id==hotelId,includeProperties:"HotelAmenity"),//HotelAmenity te modeli Hoteli , IEnumerable
                CheckInDate=checkInDate,
                Nights=nights,
                CheckOutDate=checkInDate.AddDays(nights)
            };
            booking.Total = booking.Hotel.Price * nights;
            return View(booking);
        }
    }
}
