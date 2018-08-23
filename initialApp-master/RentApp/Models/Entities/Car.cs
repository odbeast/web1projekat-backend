using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    [Table("Cars")]
    public class Car
    {
        public int Id { get; set; }
          
        public int CarYear { get; set; }
        public string Registration { get; set; }
        public int TaxiNumber { get; set; }
        public CarTypeEnum CarType { get; set; }

        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        public AppUser Driver { get; set; }
    }
}