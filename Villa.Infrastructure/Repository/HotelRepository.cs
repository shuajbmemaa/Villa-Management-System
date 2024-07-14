using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;

namespace Villa.Infrastructure.Repository
{
    public class HotelRepository : IHotelRepository
    {
        public void Add(Hotel hotel)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hotel> Get(Expression<Func<Hotel, bool>> filter, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Hotel> GetAll(Expression<Func<Hotel, bool>>? filter = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public void Remove(Hotel hotel)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Update(Hotel hotel)
        {
            throw new NotImplementedException();
        }
    }
}
