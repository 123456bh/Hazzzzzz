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
//using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfLoginProjects.Models;
using WpfLoginProjects.Services;

namespace WpfLoginProjects
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _databaseService;
        public MainWindow()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {           
            string username = UsernameTextBox.Text.Trim();
            string password = PasswordBox.Password;

            Employee loggedInEmployee = _databaseService.AuthenticateUser(username, password);
            
            if (loggedInEmployee != null)
            {
                MessageBox.Show($"Welcome, {loggedInEmployee.FullName}!", "Login Successful", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
               
                MainAppWindow mainApp = new MainAppWindow(loggedInEmployee);
                mainApp.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", 
                    MessageBoxButton.OK, MessageBoxImage.Error);

                PasswordBox.Clear();
                UsernameTextBox.Focus();
            }
        }
    }
}
