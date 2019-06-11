using lec0Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace myapp.webapi.Controllers
{
    [RoutePrefix("api/Products")]
    public class ProductsController:ApiController
    {
        private AppDbContext _context = new AppDbContext();

        [Route()]
        public IHttpActionResult Get()
        {
            try
            {
                var products = _context.Products.Select(p => new {
                    ProductName = p.Name,
                    ProductPrice = p.Price,
                    CreationDate= p.CreationDate
                });
                return Ok(products);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [Route("{name}")]
        public IHttpActionResult Get(string name)
        {
            try
            {
                var products = _context.Products.Where(p => p.Name == name).Select(p => new {
                    ProductName = p.Name,
                    ProductPrice = p.Price
                });

                if (products == null) return NotFound();

                return Ok(products);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [Route("SearchByDate/{date}/{name?}")]
        [HttpGet]
        public IHttpActionResult SearchByDate(DateTime date, string name = "")
        {
            try
            {
                var products = _context.Products.Where(p => p.CreationDate == date || p.Name.Contains(name)).Select(p => new {
                    ProductName = p.Name,
                    ProductPrice = p.Price
                }).ToList();

                if (products == null || products.Count() ==0) return NotFound();

                return Ok(products);
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [Route()]
        public IHttpActionResult Post(Product product)
        {
            try
            {
                _context.Products.Add(product);
                _context.SaveChanges();
                return Ok("Saved");
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [Route("DeleteProduct/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteProduct(int id)
        {
            try
            {
                var productEntity = _context.Products.Find(id);

                _context.Products.Remove(productEntity);
                _context.SaveChanges();
                
                return Ok("Deleted");
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}