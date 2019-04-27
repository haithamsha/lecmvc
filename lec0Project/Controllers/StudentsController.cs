using lec0Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Students
        public ActionResult Index()
        {

            return View();
        }


        public ActionResult Data()
        {
            Student student = new Models.Student();
            // get student data from data base
            var studentList = student.GetAllStudents();

            // Display data in the view
            return View(studentList);
        }

        public ActionResult Top()
        {
            // Get data from student model
            var student = new StudentModel { Id = 1, Name = "haitham" };

            return View(student);
        }
    }
}