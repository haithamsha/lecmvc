using lec0Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    [MyLogActionFilter]

    public class HomeController : Controller
    {


        
        public ActionResult Index()
        {

            return View();
        }

        
        [ActionName("AboutCompany")]
        public string About()
        {
            ViewBag.Message = "Your application description page.";

            return "from about action result";
        }

        
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