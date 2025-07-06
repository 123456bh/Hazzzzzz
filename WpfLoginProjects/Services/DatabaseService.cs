using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfLoginProjects.Models;
using System.Configuration;
using WpfLoginProjects.Helpers;

namespace WpfLoginProjects.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["MyStoreDbConnection"].ConnectionString;
        }

        //public DashboardKPIs GetDashboardKpis()
        //{
        //    // Khởi tạo một đối tượng kpis với giá trị mặc định là null.
        //    // Điều này đảm bảo rằng dù có lỗi hay không tìm thấy dữ liệu, phương thức vẫn có cái để trả về.
        //    DashboardKPIs kpis = null;

        //    // Sử dụng khối 'using' để đảm bảo kết nối được đóng tự động.
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        // Sử dụng khối 'using' cho command.
        //        using (var command = new SqlCommand("dbo.usp_GetDashboardKPIs", connection))
        //        {
        //            command.CommandType = System.Data.CommandType.StoredProcedure;

        //            try
        //            {
        //                connection.Open(); // Mở kết nối

        //                // Sử dụng khối 'using' cho reader để đảm bảo nó được đóng tự động.
        //                using (var reader = command.ExecuteReader())
        //                {
        //                    // Kiểm tra xem có hàng dữ liệu nào được trả về không.
        //                    if (reader.Read())
        //                    {
        //                        // Nếu có dữ liệu, khởi tạo đối tượng kpis và điền thông tin.
        //                        kpis = new DashboardKPIs
        //                        {
        //                            // Đọc dữ liệu từ các cột trả về bởi Stored Procedure.
        //                            // Cần chuyển kiểu dữ liệu từ object sang kiểu tương ứng (decimal, int).
        //                            TotalRevenue = (decimal)reader["TotalRevenue"],
        //                            MonthlyRevenue = (decimal)reader["MonthlyRevenue"],
        //                            TotalSales = (int)reader["TotalSales"],
        //                            TotalCustomers = (int)reader["TotalCustomers"]
        //                        };
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                // Xử lý lỗi nếu có, ví dụ: ghi log lỗi.
        //                // Trong trường hợp này, chúng ta chỉ in ra Console để debug.
        //                Console.WriteLine("Lỗi khi lấy dữ liệu KPIs: " + ex.Message);
        //                // Phương thức sẽ tiếp tục và trả về kpis (vẫn là null).
        //            }
        //        }
        //    }

        //    // Lệnh return này nằm ở cuối phương thức, đảm bảo mọi luồng xử lý đều đi qua đây.
        //    // Nếu đọc dữ liệu thành công, nó sẽ trả về đối tượng kpis có dữ liệu.
        //    // Nếu không có dữ liệu hoặc có lỗi, nó sẽ trả về giá trị null đã được khởi tạo ở đầu.
        //    return kpis;
        //}

        public Employee GetEmployeeByUsername(string username)
        {
            Employee employee = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_VerifyLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);

                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {
                                employee = new Employee
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    FullName = reader["FullName"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    PasswordHash = reader["PasswordHash"].ToString(),
                                    Salt = reader["Salt"].ToString(),
                                    RoleName = reader["RoleName"].ToString(),
                                    IsFirstLogin = Convert.ToBoolean(reader["IsFirstLogin"])
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {                
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            return employee;
        }

        // Đổi tên hàm ở đây
        public List<Product> GetAllProductsWithCategory()
        {
            var products = new List<Product>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    // Stored Procedure đã đúng, không cần sửa
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllProductsWithCategory", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new Product
                                {
                                    ProductID = Convert.ToInt32(reader["ProductID"]),
                                    ProductCode = reader["ProductCode"].ToString(),
                                    ProductName = reader["ProductName"].ToString(),
                                    CategoryID = reader["CategoryID"] as int?, // Dùng "as int?" là cách xử lý null tốt
                                    CategoryName = reader["CategoryName"].ToString(),
                                    SellingPrice = Convert.ToDecimal(reader["SellingPrice"]),
                                    CostPrice = Convert.ToDecimal(reader["CostPrice"]), // Đã có sẵn, rất tốt!
                                    InventoryQuantity = Convert.ToInt32(reader["InventoryQuantity"]),
                                    Description = reader["Description"].ToString()
                                };

                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching products: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return products;
        }

        public void AddProduct(Product product)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("usp_AddProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Thêm các tham số cho Stored Procedure
                        command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                        command.Parameters.AddWithValue("@ProductName", product.ProductName);
                        command.Parameters.AddWithValue("@CategoryID", (object)product.CategoryID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SellingPrice", product.SellingPrice);
                        command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                        command.Parameters.AddWithValue("@InventoryQuantity", product.InventoryQuantity);
                        command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);

                        // Tham số OUTPUT (không bắt buộc phải lấy giá trị trả về ở đây)
                        command.Parameters.Add("@NewProductID", SqlDbType.Int).Direction = ParameterDirection.Output;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Bắt lỗi từ SQL Server (ví dụ: trùng ProductCode)
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Ném lại lỗi để lớp gọi có thể biết thao tác thất bại
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // Hàm cập nhật sản phẩm
        public void UpdateProduct(Product product)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("usp_UpdateProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Thêm các tham số
                        command.Parameters.AddWithValue("@ProductID", product.ProductID);
                        command.Parameters.AddWithValue("@ProductCode", product.ProductCode);
                        command.Parameters.AddWithValue("@ProductName", product.ProductName);
                        command.Parameters.AddWithValue("@CategoryID", (object)product.CategoryID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@SellingPrice", product.SellingPrice);
                        command.Parameters.AddWithValue("@CostPrice", product.CostPrice);
                        command.Parameters.AddWithValue("@InventoryQuantity", product.InventoryQuantity);
                        command.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        // Hàm xóa sản phẩm
        public void DeleteProduct(int productId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    using (var command = new SqlCommand("usp_DeleteProduct", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ProductID", productId);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                // Bắt lỗi từ SQL (ví dụ: sản phẩm đã có trong đơn hàng)
                MessageBox.Show(ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }


        public List<Customer> GetAllCustomers()
        {
            var customers = new List<Customer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("usp_GetAllCustomers", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                customers.Add(new Customer
                                {
                                    CustomerID = Convert.ToInt32(reader["CustomerID"]),
                                    CustomerCode = reader["CustomerCode"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    DateRegistered = Convert.ToDateTime(reader["DateRegistered"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching customers: {ex.Message}", "Database Error");
            }
            return customers;
        }

        public List<OrderSummary> GetOrderSummaries()
        {
            var orders = new List<OrderSummary>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {                   
                    string query = "SELECT OrderID, OrderDate, CustomerName, EmployeeName, TotalAmount, OrderStatus FROM v_OrderSummaryWithDetails ORDER BY OrderDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                orders.Add(new OrderSummary
                                {
                                    OrderID = Convert.ToInt32(reader["OrderID"]),
                                    OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                    CustomerName = reader["CustomerName"].ToString(),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    OrderStatus = reader["OrderStatus"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching orders: {ex.Message}", "Database Error");
            }
            return orders;
        }
       
        public List<EmployeeViewModel> GetAllEmployeesForDisplay()
        {
            var employees = new List<EmployeeViewModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "SELECT EmployeeID, EmployeeCode, FullName, Position, Username, RoleName, PhoneNumber, Email FROM v_EmployeeDetailsWithRole ORDER BY FullName";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new EmployeeViewModel
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    EmployeeCode = reader["EmployeeCode"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Position = reader["Position"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    RoleName = reader["RoleName"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Email = reader["Email"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching employees: {ex.Message}", "Database Error");
            }
            return employees;
        }
        
        public List<Category> GetAllCategories()
        {
            var categories = new List<Category>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("usp_GetAllCategories", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                categories.Add(new Category
                                {
                                    CategoryID = (int)reader["CategoryID"],
                                    CategoryName = reader["CategoryName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching categories: {ex.Message}", "Database Error");
            }
            return categories;
        }
      
        public List<Role> GetAllRoles()
        {
            var roles = new List<Role>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {                   
                    string query = "SELECT RoleID, RoleName FROM Roles ORDER BY RoleName";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                roles.Add(new Role
                                {
                                    RoleID = (int)reader["RoleID"],
                                    RoleName = reader["RoleName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching roles: {ex.Message}", "Database Error");
            }
            return roles;
        }
        
        public List<OrderDetailViewModel> GetOrderDetailsByOrderId(int orderId)
        {
            var details = new List<OrderDetailViewModel>();
            try
            {
                string query = @"
            SELECT 
                p.ProductName, 
                od.Quantity, 
                od.UnitPrice, 
                od.Subtotal
            FROM OrderDetails od
            INNER JOIN Products p ON od.ProductID = p.ProductID
            WHERE od.OrderID = @OrderID";

                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", orderId);
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                details.Add(new OrderDetailViewModel
                                {
                                    ProductName = reader["ProductName"].ToString(),
                                    Quantity = (int)reader["Quantity"],
                                    UnitPrice = (decimal)reader["UnitPrice"],
                                    Subtotal = (decimal)reader["Subtotal"]
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching order details: {ex.Message}", "Database Error");
            }
            return details;
        }

        public bool CreateOrder(int? customerId, int employeeId, List<Product> cart, out int newOrderId)
        {
            newOrderId = 0;
            try
            {               
                DataTable orderDetailsTable = new DataTable();
                orderDetailsTable.Columns.Add("ProductID", typeof(int));
                orderDetailsTable.Columns.Add("Quantity", typeof(int));
                orderDetailsTable.Columns.Add("UnitPrice", typeof(decimal));
                
                foreach (var item in cart)
                {
                    orderDetailsTable.Rows.Add(item.ProductID, item.QuantityInCart, item.SellingPrice);
                }

                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("usp_CreateOrder", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (customerId.HasValue)
                            cmd.Parameters.AddWithValue("@CustomerID", customerId.Value);
                        else
                            cmd.Parameters.AddWithValue("@CustomerID", DBNull.Value); 

                        cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                        
                        SqlParameter detailsParam = cmd.Parameters.AddWithValue("@OrderDetails", orderDetailsTable);
                        detailsParam.SqlDbType = SqlDbType.Structured;
                        detailsParam.TypeName = "dbo.OrderDetailType"; 
                        
                        SqlParameter newIdParam = new SqlParameter("@NewOrderID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        cmd.Parameters.Add(newIdParam);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                       
                        newOrderId = (int)newIdParam.Value;
                        return true;
                    }
                }
            }
            catch (SqlException ex) 
            {
                MessageBox.Show($"Database operation failed: {ex.Message}", "Creation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public Employee AuthenticateUser(string username, string password)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {                 
                    using (var cmd = new SqlCommand("usp_VerifyLogin", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Username", username);

                        conn.Open();

                        using (var reader = cmd.ExecuteReader())
                        {                           
                            if (reader.Read())
                            {                               
                                string storedHash = reader["PasswordHash"].ToString();
                                string salt = reader["Salt"].ToString();
                                
                                if (PasswordHelper.VerifyPassword(password, storedHash, salt))
                                {                                   
                                    return new Employee
                                    {                                        
                                        EmployeeID = (int)reader["EmployeeID"],
                                        FullName = reader["FullName"].ToString(),
                                        RoleName = reader["RoleName"].ToString(),                                        
                                    };
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database authentication error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            return null;
        }

        public DashboardKPIs GetDashboardKpis()
        {
            DashboardKPIs kpis = null;
            using (var connection = new SqlConnection(_connectionString)) 
            {
                using (var command = new SqlCommand("dbo.usp_GetDashboardKPIs", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                kpis = new DashboardKPIs
                                {
                                    TotalRevenue = (decimal)reader["TotalRevenue"],
                                    MonthlyRevenue = (decimal)reader["MonthlyRevenue"],
                                    TotalSales = (int)reader["TotalSales"],
                                    TotalCustomers = (int)reader["TotalCustomers"]
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi lấy dữ liệu KPIs: " + ex.Message);
                    }
                }
            }
            return kpis;
        }

        public List<DailyRevenue> GetRevenueLast7Days()
        {
            var revenueList = new List<DailyRevenue>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.usp_GetRevenueLast7Days", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                revenueList.Add(new DailyRevenue
                                {
                                    SaleDate = (DateTime)reader["SaleDate"],
                                    Revenue = (decimal)reader["DailyRevenue"]
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi lấy doanh thu 7 ngày: " + ex.Message);
                    }
                }
            }
            return revenueList;
        }

        public List<TopSellingProduct> GetTopSellingProducts(int topN = 5)
        {
            var productList = new List<TopSellingProduct>();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand("dbo.usp_GetTopSellingProducts", connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@TopN", topN);
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productList.Add(new TopSellingProduct
                                {
                                    ProductName = reader["ProductName"].ToString(),
                                    TotalQuantitySold = (int)reader["TotalQuantitySold"]
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Lỗi khi lấy top sản phẩm: " + ex.Message);
                    }
                }
            }
            return productList;
        }

        // Thêm hàm MỚI này vào class DatabaseService
        public List<Employee> GetAllEmployeesWithRoles()
        {
            var employees = new List<Employee>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    // Sử dụng Stored Procedure đã tạo
                    using (var cmd = new SqlCommand("dbo.usp_GetAllEmployeesWithRoles", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                employees.Add(new Employee
                                {
                                    EmployeeID = Convert.ToInt32(reader["EmployeeID"]),
                                    EmployeeCode = reader["EmployeeCode"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    Position = reader["Position"].ToString(),
                                    Username = reader["Username"].ToString(),
                                    RoleID = Convert.ToInt32(reader["RoleID"]),
                                    RoleName = reader["RoleName"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    IsFirstLogin = Convert.ToBoolean(reader["IsFirstLogin"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching employees: {ex.Message}", "Database Error");
            }
            return employees;
        }

        // Dán toàn bộ các hàm này vào cuối class DatabaseService

        public void AddEmployee(Employee employee)
        {
            // Sử dụng try-catch để bắt các lỗi từ SQL Server (ví dụ: username đã tồn tại)
            try
            {
                // 'using' sẽ tự động đóng kết nối và giải phóng tài nguyên, kể cả khi có lỗi
                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand("dbo.usp_AddEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Thêm các tham số mà Stored Procedure yêu cầu
                    // (object) ... ?? DBNull.Value là cách an toàn để xử lý giá trị null
                    command.Parameters.AddWithValue("@EmployeeCode", (object)employee.EmployeeCode ?? DBNull.Value);
                    command.Parameters.AddWithValue("@FullName", employee.FullName);
                    command.Parameters.AddWithValue("@Position", (object)employee.Position ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Username", employee.Username);
                    command.Parameters.AddWithValue("@PasswordHash", employee.PasswordHash);
                    command.Parameters.AddWithValue("@Salt", employee.Salt);
                    command.Parameters.AddWithValue("@RoleID", employee.RoleID);
                    command.Parameters.AddWithValue("@PhoneNumber", (object)employee.PhoneNumber ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Email", (object)employee.Email ?? DBNull.Value);

                    // Xóa/Vô hiệu hóa dòng gây lỗi "too many arguments"
                    // Stored Procedure sẽ tự động gán IsFirstLogin = 1 cho nhân viên mới.
                    // command.Parameters.AddWithValue("@IsFirstLogin", employee.IsFirstLogin);

                    // Khai báo tham số OUTPUT để có thể lấy lại ID của nhân viên vừa được tạo
                    SqlParameter newIdParam = new SqlParameter("@NewEmployeeID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(newIdParam);

                    // Mở kết nối và thực thi lệnh
                    connection.Open();
                    command.ExecuteNonQuery();

                    // (Tùy chọn) Lấy ID của nhân viên mới và gán lại cho đối tượng employee
                    // Điều này hữu ích nếu bạn muốn cập nhật ngay lập tức giao diện
                    // mà không cần phải tải lại toàn bộ danh sách.
                    if (newIdParam.Value != DBNull.Value)
                    {
                        employee.EmployeeID = Convert.ToInt32(newIdParam.Value);
                    }
                }
            }
            catch (SqlException ex)
            {
                // Ném lại lỗi để lớp ViewModel hoặc View có thể bắt và hiển thị cho người dùng
                // Ví dụ: hiển thị MessageBox với nội dung ex.Message
                throw new Exception($"Database error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Bắt các lỗi chung khác
                throw new Exception($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_UpdateEmployee", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@EmployeeCode", (object)employee.EmployeeCode ?? DBNull.Value);
                command.Parameters.AddWithValue("@FullName", employee.FullName);
                command.Parameters.AddWithValue("@Position", (object)employee.Position ?? DBNull.Value);
                command.Parameters.AddWithValue("@Username", employee.Username);
                command.Parameters.AddWithValue("@RoleID", employee.RoleID);
                command.Parameters.AddWithValue("@PhoneNumber", (object)employee.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)employee.Email ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_DeleteEmployee", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmployeeID", employeeId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Thêm hàm này để thêm một khách hàng mới
        public void AddCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_AddCustomer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FullName", customer.FullName);
                command.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@CustomerCode", (object)customer.CustomerCode ?? DBNull.Value);

                SqlParameter newIdParam = new SqlParameter("@NewCustomerID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                command.Parameters.Add(newIdParam);

                connection.Open();
                command.ExecuteNonQuery();
                customer.CustomerID = Convert.ToInt32(newIdParam.Value);
            }
        }

        // Thêm hàm này để cập nhật thông tin khách hàng
        public void UpdateCustomer(Customer customer)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_UpdateCustomer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                command.Parameters.AddWithValue("@FullName", customer.FullName);
                command.Parameters.AddWithValue("@PhoneNumber", (object)customer.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object)customer.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@CustomerCode", (object)customer.CustomerCode ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Thêm hàm này để xóa một khách hàng
        public void DeleteCustomer(int customerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_DeleteCustomer", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CustomerID", customerId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // MỞ FILE DatabaseService.cs VÀ THÊM 3 HÀM NÀY VÀO TRONG CLASS

        // Hàm để thêm một danh mục mới
        public void AddCategory(Category category)
        {
            // Giả định rằng bạn đã có một biến _connectionString trong lớp này
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_AddCategory", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CategoryName", category.CategoryName);

                SqlParameter newIdParam = new SqlParameter("@NewCategoryID", System.Data.SqlDbType.Int)
                {
                    Direction = System.Data.ParameterDirection.Output
                };
                command.Parameters.Add(newIdParam);

                connection.Open();
                command.ExecuteNonQuery();
                category.CategoryID = Convert.ToInt32(newIdParam.Value);
            }
        }

        // Hàm để cập nhật một danh mục đã có
        public void UpdateCategory(Category category)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_UpdateCategory", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                command.Parameters.AddWithValue("@CategoryName", category.CategoryName);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Hàm để xóa một danh mục
        public void DeleteCategory(int categoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("dbo.usp_DeleteCategory", connection))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CategoryID", categoryId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }


    }
}
