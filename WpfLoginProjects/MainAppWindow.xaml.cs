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
        private readonly Employee _loggedInEmployee;
        public MainAppWindow(Employee employee)
        {
            InitializeComponent();

            _loggedInEmployee = employee;
            UserInfoText.Text = $"Welcome, {_loggedInEmployee.FullName}!";

            // Khi cửa sổ chính được mở, hãy mặc định hiển thị trang sản phẩm
            MainFrame.Navigate(new DashboardPage());
        }
       
        private void ProductMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductManagementPage());
        }
        
        private void CustomerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomerManagementPage());
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
            MainFrame.Navigate(new CategoryManagementPage());
        }
       
        private void RoleMenuItem_Click(object sender, RoutedEventArgs e)
        {            
            MainFrame.Navigate(new RoleManagementPage());
        }

        private void CreateOrderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_loggedInEmployee == null)
            {
                MessageBox.Show("Cannot create order. User information is missing.", "Error");
                return;
            }

            // Mở cửa sổ tạo đơn hàng và truyền thông tin nhân viên hiện tại vào
            CreateOrderWindow createOrderWindow = new CreateOrderWindow(_loggedInEmployee);
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
                    MainFrame.Navigate(new OrderManagementPage());
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
