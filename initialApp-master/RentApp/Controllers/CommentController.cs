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
    [RoutePrefix("api/Comment")]
    public class CommentController : ApiController
    {
        private RADBContext db;

        public CommentController(DbContext context)
        {
            db = context as RADBContext;
        }

        public IQueryable<Comment> GetLocations()
        {
            return db.Comments;
        }

        [HttpPost]
        [ResponseType(typeof(Comment))]
        [Route("AddComment")]
        public IHttpActionResult AddComment()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Comment comment = new Comment();
            var httpRequest = HttpContext.Current.Request;

            try
            {
                comment = JsonConvert.DeserializeObject<Comment>(httpRequest.Form[0]);
            }
            catch (JsonSerializationException)
            {
                return BadRequest(ModelState);
            }

            db.Comments.Add(comment);

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
        [Route("GetComment/{driveId}")]
        [ResponseType(typeof(Comment))]
        public IHttpActionResult GetComment(int driveId)
        {
            var comment = db.Comments.Where(d => d.DriveId == driveId);

            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }
    }
}
