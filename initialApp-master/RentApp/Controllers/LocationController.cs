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
    [RoutePrefix("api/Location")]
    public class LocationController : ApiController
    {
        private RADBContext db;

        public LocationController(DbContext context)
        {
            db = context as RADBContext;
        }

        public IQueryable<Location> GetLocations()
        {
            return db.Locations;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ResponseType(typeof(Location))]
        [Route("AddLocation")]
        public IHttpActionResult AddLocation()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Location location = new Location();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                location = JsonConvert.DeserializeObject<Location>(httpRequest.Form[0]);
                
            }
            catch (JsonSerializationException)
            {
                return BadRequest(ModelState);
            }

            db.Locations.Add(location);

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
