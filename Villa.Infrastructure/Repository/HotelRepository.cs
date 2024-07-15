using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villa.Application.Common.Interfaces;
using Villa.Domain.Entities;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repository
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        private readonly ApplicationDbContext _db;

        public HotelRepository(ApplicationDbContext db) : base(db)
        { 
            _db = db;
        } 

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Hotel hotel)
        {
            _db.Update(hotel);
        }
    }
}
