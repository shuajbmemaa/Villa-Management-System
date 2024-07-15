using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repository
{
    public class HotelNumberRepository : Repository<HotelNumber>, IHotelNumberRepository
    {
        private readonly ApplicationDbContext _db;

        public HotelNumberRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(HotelNumber hotelNumber)
        {
            _db.Update(hotelNumber);
        }
    }
}
