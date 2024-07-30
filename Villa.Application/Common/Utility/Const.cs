using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Application.Common.Utility
{
    public static class Const
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static int HotelRoomsAvailable(int hotelId,List<HotelNumber> hotelNumberList,
                                              DateOnly checkInDate,int nights,List<Booking> bookings)
        {
            List<int> bookingInDate = new();
            int finalAvailableRoom = int.MaxValue;

            var roomsinHotel=hotelNumberList.Where(u=>u.HotelId == hotelId).Count();

            for(int i=0;i<nights;i++)
            {
                var villasBooked = bookings.Where(u => u.CheckInDate <= checkInDate.AddDays(i) && u.CheckOutDate>=checkInDate.AddDays(i)
                                                    && u.HotelId == hotelId);

                foreach(var booking in villasBooked)
                {
                    if (!bookingInDate.Contains(booking.Id))
                    {
                        bookingInDate.Add(booking.Id);
                    }
                }

                var totalAvailableRooms=roomsinHotel - bookingInDate.Count;
                if(totalAvailableRooms == 0)
                {
                    return 0;
                }
                else
                {
                    if (finalAvailableRoom > totalAvailableRooms)
                    {
                        finalAvailableRoom = totalAvailableRooms;
                    }
                }

            }
            return finalAvailableRoom;
        }

    }
}
