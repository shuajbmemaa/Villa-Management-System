using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces
{
    public interface IHotelNumberRepository:IRepository<HotelNumber>
    {
        void Update(HotelNumber hotelNumber);
    }
}
