using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villa.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IHotelRepository Hotel { get; }
        IHotelNumberRepository HotelNumber { get; }

        void Save();

    }
}
