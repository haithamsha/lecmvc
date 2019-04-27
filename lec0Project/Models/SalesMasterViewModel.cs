using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lec0Project.Models
{
    public class SalesMasterViewModel
    {
        public int SalesId { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<SalesDetail> SalesDetails { get; set; }
    }
}