using lec0Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace myapp.webapi.Controllers
{
    [RoutePrefix("api/Categories")]

    public class CategoriesController : ApiController
    {
        private AppDbContext _context = new AppDbContext();



        [Route()]
        public IHttpActionResult Get()
        {
            try
            {
                var categories = _context.Categories.Select(p => new {
                    Name = p.Name,
                    CreationDate = p.CreationDate,
                    Description = p.Description
                });
                return Ok(categories);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [Route()]
        public IHttpActionResult Post(Category category)
        {
            try
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                return Ok("Saved");
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}
