using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using WpfLoginProjects.Models;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for CategoryManagementPage.xaml
    /// </summary>
    public partial class CategoryManagementPage : Page
    {
        private readonly DatabaseService _dbService = new DatabaseService();
        private ObservableCollection<Category> _categories;
        private Category _selectedCategory;
        private bool _isNewCategoryMode = false;

        // **THAY ĐỔI 1: Thêm thuộc tính để lưu thông tin người dùng đăng nhập**
        public Employee LoggedInEmployee { get; private set; }

        // **THAY ĐỔI 2: Sửa Constructor để nhận vào đối tượng Employee**
        public CategoryManagementPage(Employee employee)
        {
            InitializeComponent();
            _dbService = new DatabaseService();

            // Lưu lại thông tin người dùng
            this.LoggedInEmployee = employee;
            SetupPermissions();
            LoadCategories();
        }

        // **THAY ĐỔI 3: Thêm hàm mới để phân quyền chi tiết trên trang**
        private void SetupPermissions()
        {
            if (LoggedInEmployee == null || string.IsNullOrEmpty(LoggedInEmployee.RoleName))
            {
                SetControlsEnabled(false);
                return;
            }

            string roleName = LoggedInEmployee.RoleName.ToLower();

            // Theo quy tắc, chỉ Admin và Warehouse được quản lý danh mục sản phẩm
            if (roleName == "admin" || roleName == "warehouse")
            {
                SetControlsEnabled(true);
            }
            else // Các vai trò khác như Sales chỉ được xem
            {
                SetControlsEnabled(false);
            }
        }

        // **THAY ĐỔI 4: Tạo hàm tiện ích để bật/tắt các control**
        private void SetControlsEnabled(bool isEnabled)
        {
            // Giả sử các nút và textbox của bạn có tên như sau trong XAML
            btnAddNew.IsEnabled = isEnabled;
            btnSave.IsEnabled = isEnabled;
            btnDelete.IsEnabled = isEnabled;
            txtCategoryName.IsEnabled = isEnabled;
        }

        private void LoadCategories()
        {
            try
            {
                // Giả sử bạn có hàm GetAllCategories trong _dbService
                var categoriesList = _dbService.GetAllCategories();
                _categories = new ObservableCollection<Category>(categoriesList);
                CategoriesDataGrid.ItemsSource = _categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CategoriesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesDataGrid.SelectedItem is Category selectedCategory)
            {
                _isNewCategoryMode = false;
                _selectedCategory = selectedCategory;
                txtCategoryName.Text = _selectedCategory.CategoryName;
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            _isNewCategoryMode = true;
            _selectedCategory = new Category();
            CategoriesDataGrid.SelectedItem = null;
            txtCategoryName.Clear();
            txtCategoryName.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show("Category Name cannot be empty.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Đảm bảo _selectedCategory không bị null khi lưu
            if (_selectedCategory == null)
            {
                // Trường hợp này không nên xảy ra nếu logic đúng, nhưng để phòng ngừa
                btnAddNew_Click(null, null);
            }

            _selectedCategory.CategoryName = txtCategoryName.Text;

            try
            {
                if (_isNewCategoryMode)
                {
                    _dbService.AddCategory(_selectedCategory);
                    _categories.Add(_selectedCategory);
                    MessageBox.Show("Category added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (_selectedCategory.CategoryID == 0)
                    {
                        MessageBox.Show("Please select a category to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    _dbService.UpdateCategory(_selectedCategory);
                    CategoriesDataGrid.Items.Refresh();
                    MessageBox.Show("Category updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                btnAddNew_Click(null, null); // Reset form về trạng thái thêm mới
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving category: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewCategoryMode || _selectedCategory == null || _selectedCategory.CategoryID == 0)
            {
                MessageBox.Show("Please select a category from the list to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string confirmationMessage = $"Are you sure you want to delete '{_selectedCategory.CategoryName}'?";
            // Thêm cảnh báo nếu có sản phẩm đang dùng danh mục này
            // (Bạn cần thêm một hàm trong service để kiểm tra, ví dụ: IsCategoryInUse)
            // if (_dbService.IsCategoryInUse(_selectedCategory.CategoryID))
            // {
            //     confirmationMessage += "\n\nWARNING: Products in this category will become un-categorized.";
            // }

            var result = MessageBox.Show(confirmationMessage, "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteCategory(_selectedCategory.CategoryID);
                    _categories.Remove(_selectedCategory);
                    btnAddNew_Click(null, null); // Reset form
                    MessageBox.Show("Category deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting category: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
