using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<HotelNumber> HotelNumbers { get; set; }

        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Hotel>().HasData(new Hotel 
                { 
                Id = 1,
                Name = "Royal Hotel",
                Description="Hotel...",
                ImageUrl="https://placehold.co/600x400",
                Occupancy=2,
                Price=200,
                Area=60
                },
                new Hotel
                {
                    Id = 2,
                    Name = "Premium Hotel",
                    Description = "Hotely...",
                    ImageUrl = "https://placehold.co/600x401",
                    Occupancy = 4,
                    Price = 300,
                    Area = 80
                }
            );
            modelBuilder.Entity<HotelNumber>().HasData(
                new HotelNumber
                {
                    Hotel_Nr = 1,
                    HotelId=1
                },
                 new HotelNumber
                 {
                     Hotel_Nr = 2,
                     HotelId = 1
                 },
                 new HotelNumber
                 {
                     Hotel_Nr = 4,
                     HotelId = 1
                 },
                 new HotelNumber
                 {
                     Hotel_Nr = 5,
                     HotelId = 2
                 },
                 new HotelNumber
                 {
                     Hotel_Nr = 6,
                     HotelId = 2
                 },
                 new HotelNumber
                 {
                     Hotel_Nr = 7,
                     HotelId = 3
                 }

                );
            modelBuilder.Entity<Amenity>().HasData(
                new Amenity
                {
                    Id = 1,
                    HotelId = 1,
                    Name = "Private Pool"
                },
                new Amenity
                {
                    Id = 2,
                    HotelId = 2,
                    Name = "Private Balcony"
                },
                new Amenity
                {
                    Id = 3,
                    HotelId = 3,
                    Name = "Private Jacuzzi"
                }
                );
        }

    }
}
