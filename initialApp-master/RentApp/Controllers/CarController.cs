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
    [RoutePrefix("api/Car")]
    public class CarController : ApiController
    {
        private RADBContext db;

        public CarController(DbContext context)
        {
            db = context as RADBContext;
        }

        public IQueryable<Car> GetLocations()
        {
            return db.Cars;
        }

        [HttpPost]
        [ResponseType(typeof(Car))]
        [Route("AddCar")]
        public IHttpActionResult AddCar()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Car car = new Car();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                car = JsonConvert.DeserializeObject<Car>(httpRequest.Form[0]);

            }
            catch (JsonSerializationException)
            {
                return BadRequest(ModelState);
            }

            db.Cars.Add(car);

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
