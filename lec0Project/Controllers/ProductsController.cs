﻿using lec0Project.Models;
using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace lec0Project.Controllers
{
    public class ProductsController : Controller
    {
        AppDbContext _context = new AppDbContext();

        // GET: Products
        public ActionResult Index(string name, int page = 1)
        {
            IQueryable<Product> products = _context.Products;

            if (!string.IsNullOrWhiteSpace(name))
            {
                // Use productService.GetProducts();
                products = _context.Products.Where(p => p.Name.Contains(name));
            }

            else
            {
                products = _context.Products;
            }

            if(Request.IsAjaxRequest())
            {
                return PartialView("_productList", products.OrderBy(p => p.Name).ToPagedList(page, 10));
            }

            return View(products.OrderBy(p => p.Name).ToPagedList(page, 10));
        }

        public ActionResult GetProductsByName(string term)
        {
            var products = _context.Products
                .Where(p => p.Name.StartsWith(term))
                .Select(p => new
                {
                    label = p.Name,
                    id = p.ProductId
                })
                .Take(10);

            return Json(products, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult BestPrice()
        {
            var products = _context.Products.Where(p => p.Price < 30).ToList();
            return PartialView("_ProductsCategory", products);
        }

        private void FillCategoryDDL(int? categoryId =  null)
        {
            var catList = _context.Categories.ToList();

            ViewBag.catListVBag = new SelectList(catList, "CategoryId", "Name", categoryId);
        }

       



        public ActionResult Create()
        {
            // Fill Categories ddl
            FillCategoryDDL();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Product product)
        {

            try
            {
                _context.Products.Add(product);
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
            
            var product = _context.Products.Find(id);
            FillCategoryDDL(product.CategoryId);
            return View(product);
        }


        [HttpPost]
        public ActionResult Edit(int id, Product product)
        {
            try
            {
                var productEntity = _context.Products.Find(id);

                productEntity.Name = product.Name;
                productEntity.Price = product.Price;
                productEntity.Quantity = product.Quantity;
                productEntity.CategoryId = product.CategoryId;

                _context.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }


        public ActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            return View(product);
        }

        //public ActionResult Delete(int id)
        //{
        //    var product = _context.Products.Find(id);
        //    return View(product);
        //}

        //[HttpPost]
        public ActionResult Delete(int id, Product product)
        {
            try
            {
                var productEntity = _context.Products.Find(id);
                _context.Products.Remove(productEntity);
                _context.SaveChanges();
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                return View();
            }
        }

        public ActionResult InsertProduct(Product product)
        {
            product.CreationDate = DateTime.Now;
            product.UserId = User.Identity.GetUserId();
            _context.Products.Add(product);

            _context.SaveChanges();

            return Json("Data Saved!", JsonRequestBehavior.AllowGet);
        }
    }
}