using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces
{
    public interface IHotelRepository
    {
        IEnumerable<Hotel> GetAll(Expression<Func<Hotel,bool>>? filter = null,string? includeProperties = null);
        Hotel Get(Expression<Func<Hotel, bool>> filter, string? includeProperties = null);
        void Add(Hotel hotel);
        void Update(Hotel hotel);
        void Remove(Hotel hotel);
        void Save();


    }
}
