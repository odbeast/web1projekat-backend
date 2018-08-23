using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public class Driver : AppUser
    {
        [ForeignKey("Car")]
        public int? CarId { get; set; }
        public Car Car { get; set; }

        [ForeignKey("Location")]
        public int? LocationId { get; set; }
        public Location Location { get; set; }

        public string PasswordLogin { get; set; }
    }
}