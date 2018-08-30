using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using RentApp.Models.Entities;
using RentApp.Persistance;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/Driver")]
    public class DriverController : ApiController
    {
        private RADBContext db;
        public ApplicationUserManager UserManager { get; private set; }

        public DriverController(DbContext context, ApplicationUserManager userManager)
        {
        UserManager = userManager;
            db = context as RADBContext;
        }

        [Route("GetDrivers")]
        public IQueryable<AppUser> GetDrivers()
        {
            return db.AppUsers.Where(o => o.Role == Role.Driver.ToString());

        }

        [Route("GetAvailableDrivers")]
        public IQueryable<AppUser> GetAvailableDrivers()
        {
            List<int> availableDriversIds = new List<int>();
            foreach(AppUser d in db.AppUsers)
            {
                if((d as Driver) != null)
                {
                    if ((d as Driver).Available == true)
                    {
                        availableDriversIds.Add(d.Id);
                    }
                }
            }

            return db.AppUsers.Where(u => availableDriversIds.Contains(u.Id));
        }


        [HttpGet]
        [Route("GetDriver/{username}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetDriver(string username)
        {
            Driver user = (Driver)db.AppUsers
                           .Where(b => b.Username == username)
                           .FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Driver))]
        [Route("AddDriver")]
        public async Task<IHttpActionResult> AddDriver()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Driver driver = new Driver();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                driver = JsonConvert.DeserializeObject<Driver>(httpRequest.Form[0]);        
            }
            catch (JsonSerializationException)
            {
                return BadRequest(ModelState);
            }

            var user = new RAIdentityUser() { Id = driver.Username, UserName = driver.Username, Email = driver.Email, AppUser = driver, PasswordHash = RAIdentityUser.HashPassword(driver.PasswordLogin) };
            db.AppUsers.Add(driver);
            IdentityResult result = await UserManager.CreateAsync(user);
            UserManager.AddToRole(user.Id, driver.Role.ToString());

            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                return BadRequest(ModelState);
            }
            catch (DbUpdateException)
            {
                return BadRequest(ModelState);
            }

            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Driver")]
        [Route("ChangeLocation/{username}/{locationId}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult ChangeLocation(string username, int locationId)
        {
            Driver user = (Driver)db.AppUsers
                           .Where(b => b.Username == username)
                           .FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            user.LocationId = locationId;

            try
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                return BadRequest(ModelState);
            }
            catch (DbUpdateException)
            {
                return BadRequest(ModelState);
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("ChangeDriver")]
        [Authorize(Roles = "Driver")]
        [ResponseType(typeof(Driver))]
        public IHttpActionResult ChangeDriver(Driver driver)
        {
            RAIdentityUser RAuser2change = db.Users
                           .Where(b => b.UserName == driver.Username)
                           .FirstOrDefault();

            if (RAuser2change == null)
            {
                return NotFound();
            }

            Driver driver2change = (Driver)db.AppUsers
                           .Where(b => b.Id == RAuser2change.AppUserId)
                           .FirstOrDefault();

            if (driver2change == null)
            {
                return NotFound();
            }


            driver2change.ContactNumber = driver.ContactNumber;
            driver2change.DriveType = driver.DriveType;
            driver2change.Email = driver.Email;
            driver2change.FullName = driver.FullName;
            driver2change.Gender = driver.Gender;
            driver2change.DriveType = driver.DriveType;
            driver2change.LocationId = driver.LocationId;
            driver2change.CarId = driver.CarId;
            RAuser2change.PasswordHash = RAIdentityUser.HashPassword(driver.PasswordLogin);

            try
            {
                db.Entry(driver2change).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                return BadRequest(ModelState);
            }
            catch (DbUpdateException)
            {
                return BadRequest(ModelState);
            }

            return Ok(driver2change);
        }

        [HttpGet]
        [Authorize(Roles = "Driver")]
        [Route("TakeDrive/{driverId}/{id}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult TakeDrive(int driverId, int id)
        {
            Drive drive = db.Drives
                           .Where(b => b.Id == id)
                           .FirstOrDefault();
            if (drive == null)
            {
                return NotFound();
            }

            drive.DriverId = driverId;
            drive.Status = "Accepted";

            Driver driver = (Driver)db.AppUsers
                    .Where(b => b.Id == driverId)
                           .FirstOrDefault();
            driver.Available = false;

            try
            {
                db.Entry(drive).State = EntityState.Modified;
                db.Entry(driver).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (DbEntityValidationException)
            {
                return BadRequest(ModelState);
            }
            catch (DbUpdateException)
            {
                return BadRequest(ModelState);
            }

            return Ok(drive);
        }
    }
}
