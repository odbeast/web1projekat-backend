using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Persistance;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace RentApp.Controllers
{
    [RoutePrefix("api/AppUser")]
    public class AppUserController : ApiController
    {
        private RADBContext db;

        public AppUserController(DbContext context)
        {
            db = context as RADBContext;
        }

        [HttpGet]
        [Route("GetUser/{username}")]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetUser(string username)
        {
            RAIdentityUser user = db.Users
                           .Where(b => b.UserName == username)
                           .FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user.AppUserId);
        }

        [HttpGet]
        [Route("GetUserById/{id}")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult GetUserById(int id)
        {
            AppUser user = db.AppUsers
                           .Where(b => b.Id == id)
                           .FirstOrDefault();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("ChangeUser")]
        [ResponseType(typeof(AppUser))]
        public IHttpActionResult ChangeUser(RegisterBindingModel user)
        {
            RAIdentityUser RAuser2change = db.Users
                           .Where(b => b.UserName == user.Username)
                           .FirstOrDefault();

            if (RAuser2change == null)
            {
                return NotFound();
            }

            AppUser user2change = db.AppUsers
                           .Where(b => b.Id == RAuser2change.AppUserId)
                           .FirstOrDefault();

            if (user2change == null)
            {
                return NotFound();
            }


            user2change.ContactNumber = user.ContactNumber;
            user2change.DriveType = user.DriveType;
            user2change.Email = user.Email;
            user2change.FullName = user.FullName;
            user2change.Gender = user.Gender;
            user2change.DriveType = user.DriveType;
            RAuser2change.PasswordHash = RAIdentityUser.HashPassword(user.Password);

            try
            {
                db.Entry(user2change).State = EntityState.Modified;
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

            return Ok(user2change);
        }

        [HttpGet]
        [Route("GetUserDrives/{username}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult GetUserDrives(string username)
        {
            RAIdentityUser user = db.Users
                          .Where(b => b.UserName == username)
                          .FirstOrDefault();

            var drives = db.Drives.Where(d => d.CustomerId == user.AppUserId);

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        [Route("CancelDrive/{customerId}/{driveId}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult CancelDrive(int customerId, int driveId)
        {
            var drives = db.Drives.Where(b => b.CustomerId == customerId);


            if (drives == null)
            {
                return NotFound();
            }

            foreach(Drive d in drives)
            {
                if(d.Id == driveId)
                {
                    d.Status = "Canceled";
                    db.Entry(d).State = EntityState.Modified;
                }
            }

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

    }
}