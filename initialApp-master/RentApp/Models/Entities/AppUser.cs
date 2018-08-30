using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace RentApp.Models.Entities
{
    public enum Gender
    {
        None,
        Male,
        Female
    };

    public enum Role
    {
        None,
        Admin,
        Driver,
        Customer
    };

    public enum DriveType
    {
        None,
        Single,
        Multiple,
        Other
    };

    public class AppUser
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string SSN { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Role { get; set; }
        public string DriveType { get; set; }
        public string Email { get; set; }
    }
}