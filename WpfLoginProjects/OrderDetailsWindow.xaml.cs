using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfLoginProjects.Services;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for OrderDetailsWindow.xaml
    /// </summary>
    public partial class OrderDetailsWindow : Window
    {
        private readonly DatabaseService _databaseService = new DatabaseService();

        public OrderDetailsWindow(OrderSummary selectedOrder)
        {
            InitializeComponent();
            TitleTextBlock.Text = $"Details for Order #{selectedOrder.OrderID}";
            CustomerInfoTextBlock.Text = $"Customer: {selectedOrder.CustomerName} | Handled by: {selectedOrder.EmployeeName}";

            LoadDetails(selectedOrder.OrderID);
        }

        private void LoadDetails(int orderId)
        {
            OrderDetailsDataGrid.ItemsSource = _databaseService.GetOrderDetailsByOrderId(orderId);
        }
    }
}
