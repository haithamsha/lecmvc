using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class CustomersController : Controller
    {
        // GET: Customers
        public string GetAllCustomers()
        {
            return Content("Customer Name: Haitham").ToString();
        }
    }
}