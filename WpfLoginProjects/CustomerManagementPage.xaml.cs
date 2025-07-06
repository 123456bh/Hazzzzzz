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
using WpfLoginProjects.Services;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for CustomerManagementPage.xaml
    /// </summary>
    public partial class CustomerManagementPage : Page
    {
        private DatabaseService _dbService = new DatabaseService();
        private ObservableCollection<Customer> _customers;
        private Customer _selectedCustomer;
        private bool _isNewCustomerMode = false;

        public CustomerManagementPage()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            // Giả sử bạn có hàm GetAllCustomers trong _dbService
            var customersList = _dbService.GetAllCustomers();
            _customers = new ObservableCollection<Customer>(customersList);
            CustomersDataGrid.ItemsSource = _customers;
        }

        private void CustomersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _isNewCustomerMode = false;
            _selectedCustomer = CustomersDataGrid.SelectedItem as Customer;
            if (_selectedCustomer != null)
            {
                // Binding dữ liệu vào form chi tiết
                txtFullName.Text = _selectedCustomer.FullName;
                txtPhoneNumber.Text = _selectedCustomer.PhoneNumber;
                txtEmail.Text = _selectedCustomer.Email;
                txtAddress.Text = _selectedCustomer.Address;
                txtCustomerCode.Text = _selectedCustomer.CustomerCode;
            }
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            _isNewCustomerMode = true;
            _selectedCustomer = new Customer();
            CustomersDataGrid.SelectedItem = null; // Bỏ chọn trên grid

            // Xóa trắng form
            txtFullName.Text = "";
            txtPhoneNumber.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";
            txtCustomerCode.Text = "";
            txtFullName.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null)
            {
                MessageBox.Show("Please select a customer or add a new one.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Lấy dữ liệu từ form
            _selectedCustomer.FullName = txtFullName.Text;
            _selectedCustomer.PhoneNumber = txtPhoneNumber.Text;
            _selectedCustomer.Email = txtEmail.Text;
            _selectedCustomer.Address = txtAddress.Text;
            _selectedCustomer.CustomerCode = txtCustomerCode.Text;

            try
            {
                if (_isNewCustomerMode)
                {
                    // Thêm mới
                    _dbService.AddCustomer(_selectedCustomer);
                    _customers.Add(_selectedCustomer); // Cập nhật UI
                    MessageBox.Show("Customer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Cập nhật
                    _dbService.UpdateCustomer(_selectedCustomer);
                    // Cập nhật lại dòng trong DataGrid nếu cần
                    CustomersDataGrid.Items.Refresh();
                    MessageBox.Show("Customer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                _isNewCustomerMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCustomer == null || _selectedCustomer.CustomerID == 0)
            {
                MessageBox.Show("Please select a customer to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{_selectedCustomer.FullName}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _dbService.DeleteCustomer(_selectedCustomer.CustomerID);
                    _customers.Remove(_selectedCustomer); // Cập nhật UI
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    // Xóa trắng form
                    btnAddNew_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
