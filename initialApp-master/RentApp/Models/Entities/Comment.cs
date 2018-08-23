using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RentApp.Models.Entities
{
    [Table("Comments")]
    public class Comment
    {
        public int Id { get; set; }

        public string Description { get; set; }
        [Column("Date", TypeName = "datetime2")]
        public DateTime Date { get; set; }        
        public int Grade { get; set; }
        public int CustomerId { get; set; }
        public int DriveId { get; set; }
    }
}