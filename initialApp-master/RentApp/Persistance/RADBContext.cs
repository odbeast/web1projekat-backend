using Microsoft.AspNet.Identity.EntityFramework;
using RentApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace RentApp.Persistance
{
    public class RADBContext : IdentityDbContext<RAIdentityUser>
    {
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<Car> Cars { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Drive> Drives { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public DbSet<Service> Services { get; set; }

        public RADBContext() : base("name=RADB")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public static RADBContext Create()
        {
            return new RADBContext();
        }
    }
}