﻿using Newtonsoft.Json;
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
            RideStatus Status;
            Enum.TryParse(status, out Status);
            var drives = db.Drives.Where(d => d.Status == Status);

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


        [HttpGet]
        [Route("ChangeStatus/{id}/{status}")]
        [ResponseType(typeof(Drive))]
        public IHttpActionResult ChangeStatus(int id, string status)
        {
            Drive drive = db.Drives
                          .Where(b => b.Id == id)
                          .FirstOrDefault();

            if (drive == null)
            {
                return NotFound();
            }

            RideStatus myStatus;
            Enum.TryParse(status, out myStatus);
            drive.Status = myStatus;

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
            var drives = db.Drives.Where(d => d.Price >= price1 && d.Price <= price2);

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
            var comments = db.Comments.Where(c => c.Grade >= grade1 && c.Grade <= grade2);
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
            var comments = db.Comments.OrderByDescending(c => c.Grade);
            foreach (Comment c in comments)
            {
                commentIds.Add(c.DriveId);
            }
            var drives = db.Drives.Where(d => commentIds.Contains(d.Id));

            if (drives == null)
            {
                return NotFound();
            }
            return Ok(drives);
        }
    }
}
