using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {

        }

        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
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
        }

    }
}
