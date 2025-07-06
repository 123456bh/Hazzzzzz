using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace WpfLoginProjects
{
    public class OrderSummary
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }       
    }
}
