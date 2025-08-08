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
        // 1. TẠO MỘT BIẾN ĐỂ LƯU TRỮ TOÀN BỘ DANH SÁCH ĐƠN HÀNG
        private List<OrderSummary> _allOrderSummaries;

        public OrderManagementPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            // Chúng ta sẽ gọi LoadOrders từ sự kiện 'Loaded' của Page để đảm bảo UI đã sẵn sàng
        }

        // Sử dụng sự kiện Loaded của Page để tải dữ liệu (thêm Loaded="Page_Loaded" vào XAML)
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllOrders();
        }

        // Đổi tên hàm để rõ nghĩa hơn: Tải tất cả đơn hàng một lần
        private void LoadAllOrders()
        {
            // Lấy dữ liệu từ database và LƯU vào danh sách đầy đủ của chúng ta
            _allOrderSummaries = _databaseService.GetOrderSummaries();

            // Hiển thị danh sách đầy đủ lên DataGrid
            OrdersDataGrid.ItemsSource = _allOrderSummaries;
        }

        // 2. HÀM XỬ LÝ SỰ KIỆN CLICK NÚT "LỌC"
        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (FilterDatePicker.SelectedDate.HasValue)
            {
                DateTime selectedDate = FilterDatePicker.SelectedDate.Value;

                // Lọc trên danh sách đầy đủ (_allOrderSummaries), không phải trên DataGrid
                // So sánh .Date để chỉ lấy phần ngày, bỏ qua giờ/phút/giây
                var filteredList = _allOrderSummaries
                                    .Where(order => order.OrderDate.Date == selectedDate.Date)
                                    .ToList();

                // Cập nhật DataGrid với kết quả đã lọc
                OrdersDataGrid.ItemsSource = filteredList;
            }
            else
            {
                MessageBox.Show("Please select a date to filter.", "Notification");
            }
        }

        // 3. HÀM XỬ LÝ SỰ KIỆN CLICK NÚT "XÓA BỘ LỌC"
        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị lại danh sách đầy đủ ban đầu
            OrdersDataGrid.ItemsSource = _allOrderSummaries;
            // Xóa ngày đã chọn trong DatePicker
            FilterDatePicker.SelectedDate = null;
        }

        // Hàm xử lý sự kiện nháy đúp chuột của bạn (giữ nguyên)
        private void OrdersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderSummary selectedOrder)
            {
                OrderDetailsWindow detailsWindow = new OrderDetailsWindow(selectedOrder);
                detailsWindow.Owner = Window.GetWindow(this);
                detailsWindow.ShowDialog();
            }
        }
    }
}
