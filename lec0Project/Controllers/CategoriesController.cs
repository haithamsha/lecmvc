using lec0Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class CategoriesController : Controller
    {
        AppDbContext _context = new AppDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            try
            {
                // Check existing name
                if (ifExists(category.Name))
                {
                    ViewBag.Message = "Category exists before, please enter another name!";
                    return View();
                }

                // Add Category to the database
                _context.Categories.Add(category);
                _context.SaveChanges();

                ViewBag.Message = "Data Saved Successflly!";

                return View();
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        //[Route("Categories/Details/{id:regex()}")]
        public ActionResult Details(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

        public ActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }


        [HttpPost]
        public ActionResult Delete(int id, Category category)
        {
            try
            {
                var categoryEntity = _context.Categories.Find(id);
                if (categoryEntity == null) return HttpNotFound();

                // Delete from database
                _context.Categories.Remove(categoryEntity);
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }



        public ActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }


        [HttpPost]
        public ActionResult Edit(int id, Category category)
        {
            try
            {
                var categoryEntity = _context.Categories.Find(id);
                if (categoryEntity == null) return HttpNotFound();

                // Edit 
                categoryEntity.Name = category.Name;
                categoryEntity.Description = category.Description;
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }



        private bool ifExists(string name)
        {
            var result = _context.Categories.Where(c => c.Name == name);

            if(result.Count() > 0 && result != null)
            {
                return true;
            }

            return false;
        }
    }
}