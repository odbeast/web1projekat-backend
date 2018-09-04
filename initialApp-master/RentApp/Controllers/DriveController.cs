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
        [Route("GetDriveById/{id}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult GetDriveById(int id)
        {

            var drive = db.Drives.Where(d => d.Id == id);

            if (drive == null)
            {
                return NotFound();
            }
            return Ok(drive);
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

        [HttpGet]
        [Route("GetDrivesByStatus/{status}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult GetDrivesByStatus(string status)
        {
            var drives = db.Drives.Where(d => d.Status == status);

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Customer")]
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
                if(drive.DriverId != null)
                {
                    Driver user = (Driver)db.AppUsers
                               .Where(b => b.Id == drive.DriverId)
                               .FirstOrDefault();

                    user.Available = false;
                    db.Entry(user).State = EntityState.Modified;
                }
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


        [HttpGet]
        [Authorize(Roles = "Driver")]
        [Route("ChangeStatus/{id}/{status}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult ChangeStatus(int id, string status)
        {
            Drive drive = db.Drives
                          .Where(b => b.Id == id)
                          .FirstOrDefault();

            if (drive.DriverId != null)
            {
                Driver user = (Driver)db.AppUsers
                           .Where(b => b.Id == drive.DriverId)
                           .FirstOrDefault();

                user.Available = true;
                db.Entry(user).State = EntityState.Modified;
            }

            if (drive == null)
            {
                return NotFound();
            }

            drive.Status = status;

            try
            {
                db.Entry(drive).State = EntityState.Modified;
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

        [HttpGet]
        [Authorize(Roles = "Driver")]
        [Route("ChangePriceDestination/{id}/{price}/{destinationId}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult ChangePriceDestination(int id, int price, int destinationId)
        {
            Drive drive = db.Drives
                          .Where(b => b.Id == id)
                          .FirstOrDefault();

            if (drive == null)
            {
                return NotFound();
            }

            drive.Price = price;
            drive.DestinationId = destinationId;

            try
            {
                db.Entry(drive).State = EntityState.Modified;
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

        [HttpGet]
        [Route("SearchByDate/{date1}/{date2}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult SearchByDate(string date1, string date2)
        {
            DateTime t1 = DateTime.Parse(date1);
            DateTime t2 = DateTime.Parse(date1);
            var drives = db.Drives.Where(d => DateTime.Compare((d.Date), t1) >= DateTime.Compare((d.Date), t2));

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Route("SearchByPrice/{price1}/{price2}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult SearchByPrice(int price1, int price2)
        {
            List<int> ids = new List<int>();
            IQueryable<Drive> drives;

            if (price1 == -1)
            {
                drives = db.Drives.Where(d => d.Price <= price2);
            }
            else if (price2 == -1)
            {
                drives = db.Drives.Where(d => d.Price >= price1);
            }
            else
            {
                drives = db.Drives.Where(d => d.Price >= price1 && d.Price <= price2);
            }

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Route("SearchByGrade/{grade1}/{grade2}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult SearchByGrade(int grade1, int grade2)
        {
            List<int> ids = new List<int>();
            IQueryable<Comment> comments;
            if(grade1 == -1)
            {
                comments = db.Comments.Where(c => c.Grade <= grade2);
            }
            else if(grade2 == -1)
            {
                comments = db.Comments.Where(c => c.Grade >= grade1);
            }
            else
            {
                comments = db.Comments.Where(c => c.Grade >= grade1 && c.Grade <= grade2);
            }
            foreach (Comment c in comments)
            {
                ids.Add(c.DriveId);
            }

            var drives = db.Drives.Where(d => ids.Contains(d.Id));

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Route("SearchByName/{firstName}/{lastName}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult SearchByName(string firstName, string lastName)
        {
            List<int> driverIds = new List<int>();
            var drivers = db.AppUsers.Where(c => c.FullName.Contains(firstName) || c.FullName.Contains(lastName));
            foreach (AppUser c in drivers)
            {
                driverIds.Add(c.Id);
            }

            var drives = db.Drives.Where(d => driverIds.Contains(d.DriverId.Value));

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Route("SortByDate")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult SortByDate()
        {
            var drives = db.Drives.OrderByDescending(d => d.Date);

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Route("SortByGrade")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult SortByGrade()
        {
            List<int> commentIds = new List<int>();
            List<int> driveIds = new List<int>();


            var results = from drives in db.Drives
                          join comments in db.Comments on drives.CommentId equals comments.Id
                          select new
                          {
                              drives.Id,
                              drives.OriginId,
                              drives.Price,
                              drives.Status,
                              drives.DriverId,
                              drives.DestinationId,
                              drives.Date,
                              drives.CustomerId,
                              drives.CommentId,
                              drives.CarType,
                              drives.AdminId,
                              comments.Grade
                          };

            var drivesAll = results.OrderByDescending(o => o.Grade);


            if (drivesAll == null)
            {
                return NotFound();
            }
            return Ok(drivesAll);
        }
    }
}
