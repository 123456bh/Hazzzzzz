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
    /// Interaction logic for ProductManagementPage.xaml
    /// </summary>
    public partial class ProductManagementPage : Page
    {
        private readonly DatabaseService _databaseService;
        public ProductManagementPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        // Sự kiện được gọi khi trang được tải lần đầu
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProducts();
            LoadCategories();
        }

        private void LoadProducts()
        {
            try
            {
                // Gọi phương thức từ service để lấy danh sách sản phẩm (đã bao gồm CostPrice)
                List<Product> products = _databaseService.GetAllProductsWithCategory();
                ProductsDataGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadCategories()
        {
            try
            {
                // Lấy danh sách category để điền vào ComboBox
                var categories = _databaseService.GetAllCategories();
                CategoryComboBox.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Sự kiện được gọi khi người dùng chọn một sản phẩm trong DataGrid
        private void ProductsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nếu có một sản phẩm được chọn, hiển thị thông tin của nó lên form
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                ProductCodeTextBox.Text = selectedProduct.ProductCode;
                ProductNameTextBox.Text = selectedProduct.ProductName;
                SellingPriceTextBox.Text = selectedProduct.SellingPrice.ToString("F2");
                CostPriceTextBox.Text = selectedProduct.CostPrice.ToString("F2"); // Hiển thị CostPrice
                InventoryQuantityTextBox.Text = selectedProduct.InventoryQuantity.ToString();
                DescriptionTextBox.Text = selectedProduct.Description;
                CategoryComboBox.SelectedValue = selectedProduct.CategoryID;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // Xóa trắng form và bỏ chọn trong DataGrid
            ProductsDataGrid.SelectedItem = null;
            ProductCodeTextBox.Clear();
            ProductNameTextBox.Clear();
            SellingPriceTextBox.Clear();
            CostPriceTextBox.Clear(); // Xóa cả CostPrice
            InventoryQuantityTextBox.Clear();
            DescriptionTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
            ProductCodeTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validate input (bạn nên thêm kiểm tra kỹ hơn)
            if (string.IsNullOrWhiteSpace(ProductCodeTextBox.Text) ||
                string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                MessageBox.Show("Product Code and Name are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo đối tượng Product từ form
            Product product = new Product
            {
                ProductCode = ProductCodeTextBox.Text,
                ProductName = ProductNameTextBox.Text,
                CategoryID = (int?)CategoryComboBox.SelectedValue,
                Description = DescriptionTextBox.Text,
                // Chuyển đổi từ text sang decimal/int, cần try-catch để an toàn
                SellingPrice = decimal.TryParse(SellingPriceTextBox.Text, out var sp) ? sp : 0,
                CostPrice = decimal.TryParse(CostPriceTextBox.Text, out var cp) ? cp : 0, // Lấy CostPrice
                InventoryQuantity = int.TryParse(InventoryQuantityTextBox.Text, out var iq) ? iq : 0
            };

            try
            {
                // Kiểm tra xem đây là Thêm mới hay Cập nhật
                if (ProductsDataGrid.SelectedItem is Product selectedProduct)
                {
                    // Đây là Cập nhật
                    product.ProductID = selectedProduct.ProductID;
                    _databaseService.UpdateProduct(product);
                    MessageBox.Show("Product updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Đây là Thêm mới
                    _databaseService.AddProduct(product);
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Tải lại danh sách và xóa form
                LoadProducts();
                ClearButton_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // Chỉ xóa khi có một sản phẩm được chọn
            if (ProductsDataGrid.SelectedItem is Product selectedProduct)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{selectedProduct.ProductName}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _databaseService.DeleteProduct(selectedProduct.ProductID);
                        MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadProducts();
                        ClearButton_Click(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
