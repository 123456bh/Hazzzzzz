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
    /// Interaction logic for RoleManagementPage.xaml
    /// </summary>
    public partial class RoleManagementPage : Page
    {
        // Tạo một đối tượng dịch vụ database để sử dụng
        private readonly DatabaseService _databaseService = new DatabaseService();

        // Hàm này được gọi ngay khi trang được tạo ra
        public RoleManagementPage()
        {
            InitializeComponent();

            // Gọi hàm để tải dữ liệu lên bảng
            LoadRoles();
        }

        // Hàm chuyên để lấy dữ liệu và hiển thị lên DataGrid
        private void LoadRoles()
        {
            // 1. Gọi phương thức GetAllRoles() từ DatabaseService để lấy danh sách
            List<Role> rolesList = _databaseService.GetAllRoles();

            // 2. Gán danh sách vừa lấy được làm nguồn dữ liệu cho DataGrid
            //    WPF sẽ tự động đọc danh sách và hiển thị lên các cột theo Binding đã thiết lập
            RolesDataGrid.ItemsSource = rolesList;
        }
    }
}
