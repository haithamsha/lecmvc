using lec0Project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class CategoriesController : Controller
    {
        AppDbContext _context = new AppDbContext();

        // GET: Categories
        public async Task<ActionResult> Index()
        {
            string apiUrl = "http://localhost:47719/api/Categories";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage catApi = await client.GetAsync(apiUrl);

                if (catApi.IsSuccessStatusCode)
                {
                    var response = await catApi.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<List<Category>>(response);
                    return View(data);
                }

                return View();
            }
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

                // Upload file
                if(category.File != null)
                {
                    category.File.SaveAs(Server.MapPath($"~/Content/{category.File.FileName}"));
                    category.FileName = category.File.FileName;
                }

                // Add Category to the database
                //_context.Categories.Add(category);
                //_context.SaveChanges();

                #region Save With Api

                string apiUrl = "http://localhost:47719/api/Categories";

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);

                    client.DefaultRequestHeaders.Clear();
                    var postTask = client.PostAsJsonAsync<Category>(apiUrl, category);
                    

                    postTask.Wait();
                    var result = postTask.Result;

                    //Checking the response is successful or not which is sent using HttpClient  
                    if (result.IsSuccessStatusCode)
                    {
                        fillDropDownLists();
                        var _response = result.Content.ReadAsStringAsync().Result;
                        var _msg = JsonConvert.DeserializeObject<object>(_response);
                        ViewBag.resultMessage = _msg;
                        return View(vm);

                    }
                }
                
                #endregion



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