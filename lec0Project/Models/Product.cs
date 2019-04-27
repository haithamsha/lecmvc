using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace lec0Project.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Display(Name= "Product Name")]
        [StringLength(50, MinimumLength =5)]
        public string Name { get; set; }
        [Range(1, 1000, ErrorMessage ="يجب ان تكون القيمة من 1 الي 1000")]
        public decimal Quantity { get; set; }
        public decimal? Price { get; set; }

        // Lazy Loading
        public virtual Category Category { get; set; }
        [Display(Name="Category")]

        public int CategoryId { get; set; }
        
    }
}