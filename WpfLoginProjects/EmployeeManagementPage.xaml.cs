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
using WpfLoginProjects.Helpers;
using WpfLoginProjects.Models;
using WpfLoginProjects.Services;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for EmployeeManagementPage.xaml
    /// </summary>
    public partial class EmployeeManagementPage : Page
    {
        private readonly DatabaseService _databaseService;

        public EmployeeManagementPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadEmployees();
            LoadRoles();
        }

        private void LoadEmployees()
        {
            try
            {
                var employees = _databaseService.GetAllEmployeesWithRoles();
                EmployeesDataGrid.ItemsSource = employees;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employees: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRoles()
        {
            try
            {
                var roles = _databaseService.GetAllRoles(); // Bạn cần tạo hàm này trong DatabaseService
                RoleComboBox.ItemsSource = roles;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading roles: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee selectedEmployee)
            {
                EmployeeCodeTextBox.Text = selectedEmployee.EmployeeCode;
                FullNameTextBox.Text = selectedEmployee.FullName;
                PositionTextBox.Text = selectedEmployee.Position;
                UsernameTextBox.Text = selectedEmployee.Username;
                EmailTextBox.Text = selectedEmployee.Email;
                PhoneNumberTextBox.Text = selectedEmployee.PhoneNumber;
                RoleComboBox.SelectedValue = selectedEmployee.RoleID;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeesDataGrid.SelectedItem = null;
            EmployeeCodeTextBox.Clear();
            FullNameTextBox.Clear();
            PositionTextBox.Clear();
            UsernameTextBox.Clear();
            EmailTextBox.Clear();
            PhoneNumberTextBox.Clear();
            RoleComboBox.SelectedIndex = -1;
            EmployeeCodeTextBox.Focus();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                RoleComboBox.SelectedValue == null)
            {
                MessageBox.Show("Username, Full Name, and Role are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Employee employee = new Employee
            {
                EmployeeCode = EmployeeCodeTextBox.Text,
                FullName = FullNameTextBox.Text,
                Position = PositionTextBox.Text,
                Username = UsernameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text,
                RoleID = (int)RoleComboBox.SelectedValue
            };

            try
            {
                if (EmployeesDataGrid.SelectedItem is Employee selectedEmployee)
                {
                    // Update existing employee
                    employee.EmployeeID = selectedEmployee.EmployeeID;
                    _databaseService.UpdateEmployee(employee); // Bạn cần tạo hàm này
                    MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // Add new employee
                    // Set a default password for new users
                    string defaultPassword = "Password123!"; // Hoặc tạo một mật khẩu ngẫu nhiên
                    employee.Salt = PasswordHelper.GenerateSalt();
                    employee.PasswordHash = PasswordHelper.HashPassword(defaultPassword, employee.Salt);
                    employee.IsFirstLogin = true; // Bắt buộc đổi mật khẩu khi đăng nhập lần đầu

                    _databaseService.AddEmployee(employee); // Bạn cần tạo hàm này
                    MessageBox.Show($"Employee added successfully! Default password is: {defaultPassword}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadEmployees();
                ClearButton_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show($"Are you sure you want to delete employee '{selectedEmployee.FullName}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _databaseService.DeleteEmployee(selectedEmployee.EmployeeID); // Bạn cần tạo hàm này
                        MessageBox.Show("Employee deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadEmployees();
                        ClearButton_Click(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting employee: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
