﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    public enum RideStatus
    {
        Created = 0,
        Canceled = 1,
        Formed = 2,
        Processed = 3,
        Accepted = 4,
        Unsuccessfull,
        Successfull
    };

    [Table("Drives")]
    public class Drive
    {
        public int Id { get; set; }

        [Column("Date", TypeName = "datetime2")]
        public DateTime Date { get; set; }
        public CarTypeEnum CarType { get; set; } 
        public float Price { get; set; }
        //public Comment Comment { get; set; } //TODO: Check the ForeingKey.
        public RideStatus Status { get; set; }

        [ForeignKey("Origin")]
        public int? OriginId { get; set; }
        public Location Origin { get; set; }
        [ForeignKey("Destination")]
        public int? DestinationId { get; set; }
        public Location Destination { get; set; }
        [ForeignKey("Admin")]
        public int? AdminId { get; set; }
        public AppUser Admin { get; set; }
        [ForeignKey("Driver")]
        public int? DriverId { get; set; }
        public Driver Driver { get; set; }

    }
}