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
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/Drive")]
    public class DriveController : ApiController
    {
        private RADBContext db;

        public DriveController(DbContext context)
        {
            db = context as RADBContext;
        }


        public IQueryable<Drive> GetDrives()
        {
            return db.Drives;
        }


        [HttpGet]
        [Route("GetAdminDrives/{username}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult GetAdminDrives(string username)
        {
            RAIdentityUser user = db.Users
                          .Where(b => b.UserName == username)
                          .FirstOrDefault();

            var drives = db.Drives.Where(d => d.AdminId == user.AppUserId);

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Route("GetDriverDrives/{username}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult GetDriverDrives(string username)
        {
            RAIdentityUser user = db.Users
                          .Where(b => b.UserName == username)
                          .FirstOrDefault();

            var drives = db.Drives.Where(d => d.DriverId == user.AppUserId);

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpPost]
        [ResponseType(typeof(Drive))]
        [Route("AddDrive")]
        public IHttpActionResult AddDrive()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Drive drive = new Drive();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                drive = JsonConvert.DeserializeObject<Drive>(httpRequest.Form[0]);
            }
            catch (JsonSerializationException)
            {
                return BadRequest(ModelState);
            }

            db.Drives.Add(drive);

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

            return Ok("Success");
        }
    }
}
