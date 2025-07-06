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
using System.Windows.Shapes;
using WpfLoginProjects.Models;
using WpfLoginProjects.Services;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for CreateOrderWindow.xaml
    /// </summary>
    public partial class CreateOrderWindow : Window
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private readonly Employee _currentUser;
        private List<Product> _allProducts; // Lưu danh sách tất cả sản phẩm
        private ObservableCollection<Product> _cartItems = new ObservableCollection<Product>();

        public CreateOrderWindow(Employee currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            this.DataContext = this; // Cần cho binding sau này
            CartDataGrid.ItemsSource = _cartItems;
            LoadInitialData();
            ProductSearchTextBox.TextChanged += ProductSearchTextBox_TextChanged;
        }

        private void LoadInitialData()
        {
            _allProducts = _databaseService.GetAllProductsWithCategory();
            ProductListBox.ItemsSource = _allProducts;
            CustomerComboBox.ItemsSource = _databaseService.GetAllCustomers();
        }

        private void ProductSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = ProductSearchTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ProductListBox.ItemsSource = _allProducts;
            }
            else
            {
                ProductListBox.ItemsSource = _allProducts.Where(p => p.ProductName.ToLower().Contains(searchText));
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProductListBox.SelectedItem is Product selectedProduct)
            {
                // Tìm xem sản phẩm đã có trong giỏ hàng chưa
                var itemInCart = _cartItems.FirstOrDefault(p => p.ProductID == selectedProduct.ProductID);
                if (itemInCart != null)
                {
                    itemInCart.QuantityInCart++;
                    CartDataGrid.Items.Refresh(); // Cần refresh để DataGrid cập nhật
                }
                else
                {
                    // Tạo một bản sao để không ảnh hưởng đến danh sách gốc
                    var productToAdd = new Product
                    {
                        ProductID = selectedProduct.ProductID,
                        ProductName = selectedProduct.ProductName,
                        SellingPrice = selectedProduct.SellingPrice,
                        QuantityInCart = 1
                    };
                    _cartItems.Add(productToAdd);
                }
                UpdateTotal();
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Product itemToRemove)
            {
                _cartItems.Remove(itemToRemove);
                UpdateTotal();
            }
        }

        private void UpdateTotal()
        {
            decimal total = _cartItems.Sum(p => p.SellingPrice * p.QuantityInCart);
            TotalAmountText.Text = $"Total: {total:C}";
        }

        private void SaveOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("The shopping cart is empty.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy CustomerID, có thể là null
            int? customerId = (CustomerComboBox.SelectedItem as Customer)?.CustomerID;

            // Gọi service để tạo đơn hàng
            bool success = _databaseService.CreateOrder(customerId, _currentUser.EmployeeID, _cartItems.ToList(), out int newOrderId);

            if (success)
            {
                MessageBox.Show($"Order #{newOrderId} has been created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true; // Báo cho cửa sổ cha biết là đã thành công
                this.Close();
            }
            // Nếu không thành công, DatabaseService đã hiện MessageBox lỗi rồi
        }
    }
}
