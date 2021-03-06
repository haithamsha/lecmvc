﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace lec0Project.Models
{
    public class Category:SharedUserData
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name Is Required!")]
        [StringLength(50)]
        public string Name { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }

        [NotMapped]
        public HttpPostedFileBase File { get; set; }

        public virtual ICollection<Product> Products { get; set; }

    }
}