using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
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
        public IActionResult Index()
        {
            return View();
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
        public IActionResult BookingDetails(int bookingId)
        {
            Booking bookingfromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Hotel");

            if(bookingfromDb.VillaNumber == 0 && bookingfromDb.Status == Const.StatusApproved)
            {
                var availableHotelNr=AvailableHotelNumber(bookingfromDb.HotelId);

                bookingfromDb.HotelNumbers = _unitOfWork.HotelNumber.GetAll(u => u.HotelId == bookingfromDb.HotelId
                && availableHotelNr.Any(x=>x==u.Hotel_Nr)).ToList();
            }

            return View(bookingfromDb); 
        }

        private List<int> AvailableHotelNumber(int hotelId)
        {
            List<int> availableHotelNumbers = new();
            var hotelNumber = _unitOfWork.HotelNumber.GetAll(u => u.HotelId == hotelId);

            var checkedHotel=_unitOfWork.Booking.GetAll(u=> u.Status == Const.StatusCheckedIn && u.HotelId == hotelId)
                .Select(u=>u.VillaNumber);

            foreach(var hotelNr in hotelNumber)
            {
                if(!checkedHotel.Contains(hotelNr.Hotel_Nr))
                {
                    availableHotelNumbers.Add(hotelNr.Hotel_Nr);
                }
            }
            return availableHotelNumbers;
        }

        [HttpPost]
        [Authorize(Roles = Const.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, Const.StatusCheckedIn,booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "Booking was updated successfully";
            return RedirectToAction(nameof(BookingDetails), new {bookingId =  booking.Id});
        }

        [HttpPost]
        [Authorize(Roles = Const.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, Const.StatusCompleted, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["Success"] = "Booking was completed successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [HttpPost]
        [Authorize(Roles = Const.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _unitOfWork.Booking.UpdateStatus(booking.Id, Const.StatusCancelled,0);
            _unitOfWork.Save();
            TempData["Success"] = "Booking was cancelled successfully";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }

        [Authorize]
        [HttpPost]
        public IActionResult FinalizeBooking(Booking booking)
        {
            var hotel = _unitOfWork.Hotel.Get(y => y.Id == booking.HotelId);
            booking.Total =  hotel.Price * booking.Nights;
            booking.Status=Const.StatusPending;
            booking.BookingDate = DateTime.Now;

            var hotelNumberList = _unitOfWork.HotelNumber.GetAll().ToList();
            var bookedHotels = _unitOfWork.Booking.GetAll(u => u.Status == Const.StatusApproved || u.Status == Const.StatusCheckedIn).ToList();

            
            int roomAvailable = Const.HotelRoomsAvailable(hotel.Id, hotelNumberList, booking.CheckInDate, booking.Nights, bookedHotels);

            if(roomAvailable == 0)
            {
                TempData["error"] = "Dhoma eshte shitur !!";
                return RedirectToAction(nameof(FinalizeBooking), new
                {
                    hotelId = booking.HotelId,
                    checkInDate = booking.CheckInDate,
                    nights = booking.Nights
                });
            }            

            _unitOfWork.Booking.Add(booking);
            _unitOfWork.Save();

            var domain = Request.Scheme + "://" + Request.Host.Value+"/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"booking/BookingConfirmation?bookingId={booking.Id}",
                CancelUrl = domain + $"booking/FinalizeBooking?hotelId={booking.HotelId}&checkInDate={booking.CheckInDate}&nights={booking.Nights}",
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.Total * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = hotel.Name,
                        //Images = new List<string> { domain + hotel.ImageUrl },
                    },
                },
                Quantity=1,
            });

            
            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.Booking.UpdateStripePaymentID(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

           //return RedirectToAction(nameof(BookingConfirmation),new { bookingId = booking.Id}); // new bookingId duhet me kan e njejt si parametri te metoda BookingConfirmation
        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingfromDb = _unitOfWork.Booking.Get(u => u.Id == bookingId, includeProperties: "User,Hotel");

            if (bookingfromDb.Status == Const.StatusPending)
            {
                var service=new SessionService();
                Session session = service.Get(bookingfromDb.StripeSessionId);
                if(session.PaymentStatus == "paid") {
                    //bookingfromDb.Status = Const.StatusApproved;
                    _unitOfWork.Booking.UpdateStatus(bookingfromDb.Id, Const.StatusApproved,0);
                    _unitOfWork.Booking.UpdateStripePaymentID(bookingfromDb.Id,session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
           
            return View(bookingId);
        }

        #region API Calls
        [HttpGet]
        [Authorize]
        public IActionResult GetAll(string status)
        {
            

            IEnumerable<Booking> objBookings;
            if (User.IsInRole(Const.Role_Admin))
            {
                objBookings = _unitOfWork.Booking.GetAll(includeProperties: "User,Hotel");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                objBookings = _unitOfWork.Booking.GetAll(u => u.UserId == userId, includeProperties: "User,Hotel");
            }

            if(!string.IsNullOrEmpty(status))
            {
                objBookings = objBookings.Where(u => u.Status.ToLower().Equals(status.ToLower()));
                //objBookings = objBookings.Where(u => u.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            }

            return Json(new {data=objBookings});
        }

        #endregion
    }
}
