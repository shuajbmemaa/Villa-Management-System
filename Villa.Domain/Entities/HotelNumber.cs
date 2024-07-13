using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villa.Domain.Entities
{
    public class HotelNumber
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Hotel_Nr { get; set; }

        [ForeignKey("Hotel")]
        public int HotelId { get; set; }
        public Hotel Hotel { get; set; }
        public string? SpecialDetails { get; set; }
    }
}
