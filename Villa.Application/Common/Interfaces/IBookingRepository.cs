using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {

        void Update(Booking booking);
        void UpdateStatus(int bokingId,string status);
        void UpdateStripePaymentID(int bookingId,string sessionId,string paymentIntentId);

    }
}
