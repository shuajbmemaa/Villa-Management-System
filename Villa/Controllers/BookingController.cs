﻿using Microsoft.AspNetCore.Authorization;
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
                    _unitOfWork.Booking.UpdateStatus(bookingfromDb.Id, Const.StatusApproved);
                    _unitOfWork.Booking.UpdateStripePaymentID(bookingfromDb.Id,session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }
           
            return View(bookingId);
        }
    }
}
