using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class MoviesController : Controller
    {
        // GET: Movies
        public ActionResult Index(int? pageIndex, string sortBy)
        {
            if (!pageIndex.HasValue)
                pageIndex = 1;

            if (string.IsNullOrWhiteSpace(sortBy))
                sortBy = "Name";

            return Content($"Page Index: {pageIndex}, sort by: {sortBy}");
        }
        
        public ActionResult Edit(int id)
        {
            return Content($"Id: {id}");
        }

        // Movies/2019/1
        [Route("movies/ByDate/{year}/{month:range(1, 12)}")]
        public ActionResult ByDate(int year, int? month)
        {
            return Content($"{year}/{month}");
        }
    }
}