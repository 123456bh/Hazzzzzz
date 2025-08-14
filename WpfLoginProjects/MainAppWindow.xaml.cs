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
using WpfLoginProjects.Models;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        // Đổi tên biến để rõ ràng hơn, đây là thông tin của người dùng đã đăng nhập
        public Employee LoggedInEmployee { get; private set; }

        public MainAppWindow(Employee employee)
        {
            InitializeComponent();

            LoggedInEmployee = employee;
            WelcomeMessageText.Text = $"Welcome, {LoggedInEmployee.FullName} ({LoggedInEmployee.RoleName})!";

            // *** BƯỚC QUAN TRỌNG: Thiết lập giao diện dựa trên vai trò ***
            SetupUIVisibilityByRole();

            // Khi cửa sổ chính được mở, hãy mặc định hiển thị trang sản phẩm
            MainFrame.Navigate(new DashboardPage());
        }

        // *** HÀM MỚI: Logic để ẩn/hiện các thành phần giao diện ***
        private void SetupUIVisibilityByRole()
        {
            // Kiểm tra xem thông tin vai trò có hợp lệ không
            if (LoggedInEmployee == null || string.IsNullOrEmpty(LoggedInEmployee.RoleName))
            {
                // Nếu không có vai trò, ẩn hết các chức năng nhạy cảm để đảm bảo an toàn
                MenuCustomers.Visibility = Visibility.Collapsed;
                MenuOrders.Visibility = Visibility.Collapsed;
                MenuEmployees.Visibility = Visibility.Collapsed;
                MenuRoles.Visibility = Visibility.Collapsed;
                MenuCreateOrder.Visibility = Visibility.Collapsed; // Giả sử MenuItem tạo đơn hàng có tên là MenuCreateOrder
                return;
            }

            string roleName = LoggedInEmployee.RoleName.ToLower();

            // Sử dụng switch-case để quản lý quyền cho từng vai trò
            switch (roleName)
            {
                case "admin":
                    // Admin có toàn quyền, không cần ẩn gì.
                    break;

                case "sales":
                    // Vai trò Sales: Ẩn các chức năng quản lý nhân sự và vai trò
                    MenuEmployees.Visibility = Visibility.Collapsed;
                    MenuRoles.Visibility = Visibility.Collapsed;
                    //MenuCreateOrder.Visibility = Visibility.Collapsed;
                    break;

                case "warehouse":
                    // Vai trò Warehouse: Chỉ quản lý sản phẩm, ẩn các chức năng liên quan đến bán hàng và khách hàng
                    MenuCreateOrder.Visibility = Visibility.Collapsed;
                    MenuCustomers.Visibility = Visibility.Collapsed;
                    MenuOrders.Visibility = Visibility.Collapsed;
                    MenuEmployees.Visibility = Visibility.Collapsed;
                    MenuRoles.Visibility = Visibility.Collapsed;
                    break;

                default:
                    // Mặc định cho các vai trò khác (nếu có): ẩn các chức năng nhạy cảm
                    MenuEmployees.Visibility = Visibility.Collapsed;
                    MenuRoles.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void ProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductManagementPage(LoggedInEmployee));
        }
        
        private void CustomerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomerManagementPage(LoggedInEmployee));
        }
        
        private void OrderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrderManagementPage());
        }
        
        private void EmployeeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeeManagementPage());
        }
       
        private void CategoryMenuItem_Click(object sender, RoutedEventArgs e)
        {            
            MainFrame.Navigate(new CategoryManagementPage(LoggedInEmployee));
        }
       
        private void RoleMenuItem_Click(object sender, RoutedEventArgs e)
        {            
            MainFrame.Navigate(new RoleManagementPage());
        }

        private void CreateOrderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (LoggedInEmployee == null)
            {
                MessageBox.Show("Cannot create order. User information is missing.", "Error");
                return;
            }

            // Mở cửa sổ tạo đơn hàng và truyền thông tin nhân viên hiện tại vào
            CreateOrderWindow createOrderWindow = new CreateOrderWindow(LoggedInEmployee);
            createOrderWindow.Owner = this; // Đặt cửa sổ chính làm chủ

            // ShowDialog sẽ đợi cho đến khi cửa sổ tạo đơn hàng được đóng
            // và trả về true nếu đơn hàng được tạo thành công
            bool? result = createOrderWindow.ShowDialog();

            // Nếu đơn hàng được tạo thành công, làm mới danh sách đơn hàng
            if (result == true)
            {
                // Nếu đang ở trang Orders, hãy tải lại nó
                if (MainFrame.Content is OrderManagementPage orderPage)
                {
                    MainFrame.Navigate(new OrderManagementPage(
                        ));
                }
            }
        }

        private void DashboardMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // Tạm thời hiển thị MessageBox vì chưa tạo trang Dashboard
            MainFrame.Navigate(new DashboardPage());
            // Sau này sẽ thay bằng: MainFrame.Navigate(new DashboardPage());
        }

        private void LogoutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow loginWindow = new MainWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "Are you sure you want to exit the application?", "Confirm Exit",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
