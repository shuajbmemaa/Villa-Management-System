using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villa.Domain.Entities
{
    public class Hotel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Price per night")]
        [Range(10,1000)]
        public double Price { get; set; }
        public int Area { get; set; }
        [Range(1,10)]
        public int Occupancy { get; set; }
        [NotMapped] //data anotation qe nuk e shton atributin ne DB
        public IFormFile? Image { get; set; }
        [Display(Name ="Image Url")]
        public string? ImageUrl { get; set; }
        public DateTime? Created_Date { get; set; }
        public DateTime? Updated_Date { get;set; }

        [ValidateNever]
        public IEnumerable<Amenity> HotelAmenity { get; set; }

    }
}
