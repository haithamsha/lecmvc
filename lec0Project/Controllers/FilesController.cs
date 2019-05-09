using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            file.SaveAs(Server.MapPath($"~/Content/{file.FileName}"));
            ViewBag.message = "File uploaded successfully!";
            return View();
        }

        public ActionResult Download()
        {
            // Read file
            byte[] bytes = System.IO.File.ReadAllBytes(Server.MapPath("~/Content/myfile.txt"));

            // Download
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, "myfile.txt");
        }
    }
}