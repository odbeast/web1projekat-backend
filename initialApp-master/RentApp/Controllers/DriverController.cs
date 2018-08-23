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

        public IQueryable<AppUser> GetDrivers()
        {
            return db.AppUsers.Where(o => o.Role == Role.Driver.ToString());
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
    }
}
