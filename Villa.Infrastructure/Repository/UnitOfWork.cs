using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villa.Application.Common.Interfaces;
using Villa.Infrastructure.Data;

namespace Villa.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IHotelRepository Hotel {  get; private set; }

        public IHotelNumberRepository HotelNumber {  get; private set; }

        public IAmenityRepository Amenity { get; private set; }
        
        public IBookingRepository Booking { get; private set; }
        public IUserRepository User { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db=db;
            Hotel = new HotelRepository(_db);
            HotelNumber= new HotelNumberRepository(_db);
            Amenity = new AmenityRepository(_db);
            Booking = new BookingRepository(_db);
            User = new UserRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
