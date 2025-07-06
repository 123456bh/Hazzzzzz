using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfLoginProjects
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int? CategoryID { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal CostPrice { get; set; }
        public int InventoryQuantity { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }               
        public int QuantityInCart { get; set; }
    }
}
