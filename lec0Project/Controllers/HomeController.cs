using lec0Project.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace lec0Project.Controllers
{
    [MyLogActionFilter]

    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {

        
        
        public ActionResult Index()
        {
            ViewBag.userName = User.Identity.Name;
            var userId = User.Identity.GetUserId();
            return View();
        }

        
        [ActionName("AboutCompany")]
        public string About()
        {
            ViewBag.Message = "Your application description page.";

            return "from about action result";
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(string name, string phone)
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}