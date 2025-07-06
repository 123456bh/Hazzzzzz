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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfLoginProjects.Services;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for OrderManagementPage.xaml
    /// </summary>
    public partial class OrderManagementPage : Page
    {
        private readonly DatabaseService _databaseService;

        public OrderManagementPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            LoadOrders();
        }

        private void LoadOrders()
        {
            OrdersDataGrid.ItemsSource = _databaseService.GetOrderSummaries();
        }

        // HÀM MỚI ĐỂ XỬ LÝ SỰ KIỆN NHÁY ĐÚP CHUỘT
        private void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Kiểm tra xem người dùng có thực sự click vào một dòng có dữ liệu hay không
            if (OrdersDataGrid.SelectedItem is OrderSummary selectedOrder)
            {
                // 1. Tạo một cửa sổ chi tiết đơn hàng mới, truyền đối tượng đơn hàng đã chọn vào
                OrderDetailsWindow detailsWindow = new OrderDetailsWindow(selectedOrder);

                // 2. (Tùy chọn nhưng nên có) Đặt cửa sổ chính làm chủ của cửa sổ pop-up.
                // Điều này giúp cửa sổ pop-up luôn nằm trên cửa sổ chính.
                detailsWindow.Owner = Window.GetWindow(this);

                // 3. Hiển thị cửa sổ chi tiết. 
                // ShowDialog() sẽ khóa cửa sổ chính lại cho đến khi cửa sổ chi tiết được đóng.
                detailsWindow.ShowDialog();
            }
        }

    }
}
