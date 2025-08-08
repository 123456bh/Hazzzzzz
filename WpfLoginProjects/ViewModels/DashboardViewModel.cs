using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Wpf;
using WpfLoginProjects.Models;
using WpfLoginProjects.Services;

namespace WpfLoginProjects.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        // --- Fields ---
        private readonly DatabaseService _dbService;

        // --- Properties for Data Binding ---

        // Collection để lưu các ô chỉ số KPI (Tổng doanh thu, etc.)
        public ObservableCollection<KpiItem> KpiItems { get; set; }

        // Collection để lưu danh sách top sản phẩm bán chạy
        public ObservableCollection<TopSellingProduct> TopProducts { get; set; }

        // --- Properties cho thư viện biểu đồ LiveCharts ---

        // Dữ liệu cho các cột/đường trong biểu đồ
        public SeriesCollection RevenueSeries { get; set; }

        // Nhãn cho trục hoành (trục X) của biểu đồ, ví dụ: "20/06", "21/06"
        public string[] RevenueLabels { get; set; }

        // Một hàm để định dạng các nhãn trên trục tung (trục Y), ví dụ: "1,200,000"
        public Func<double, string> YFormatter { get; set; }

        // --- Constructor ---
        public DashboardViewModel()
        {
            // Khởi tạo các đối tượng và collection trước
            KpiItems = new ObservableCollection<KpiItem>();
            TopProducts = new ObservableCollection<TopSellingProduct>();
            RevenueSeries = new SeriesCollection();
            YFormatter = value => value.ToString("N0");

            // Kiểm tra xem có đang ở chế độ thiết kế của Visual Studio hay không
            bool isInDesignMode = DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());

            if (isInDesignMode)
            {
                // Nếu đang ở chế độ thiết kế, chúng ta có thể tải dữ liệu giả để xem trước
                // (Phần này là tùy chọn, nhưng rất hữu ích)
                LoadSampleDataForDesigner();
            }
            else
            {
                // Nếu đang chạy ứng dụng thật, mới khởi tạo DatabaseService và tải dữ liệu thật
                _dbService = new DatabaseService();
                LoadDashboardData();
            }
        }

        // THÊM PHƯƠNG THỨC MỚI NÀY (Tùy chọn, để xem trước)
        /// <summary>
        /// Tải dữ liệu giả để hiển thị trong cửa sổ Designer.
        /// </summary>
        private void LoadSampleDataForDesigner()
        {
            // Dữ liệu giả cho KPI
            KpiItems.Add(new KpiItem { Title = "Total Revenue", Value = "123,456,789 VNĐ", Icon = "" });
            KpiItems.Add(new KpiItem { Title = "This Month's Revenue", Value = "12,345,678 VNĐ", Icon = "" });
            KpiItems.Add(new KpiItem { Title = "Total Order", Value = "1,234", Icon = "" });
            KpiItems.Add(new KpiItem { Title = "Total Customers", Value = "567", Icon = "" });

            // Dữ liệu giả cho Top Products
            TopProducts.Add(new TopSellingProduct { ProductName = "iPhone 15 Pro Max (Sample)", TotalQuantitySold = 150 });
            TopProducts.Add(new TopSellingProduct { ProductName = "Macbook Air M2 (Sample)", TotalQuantitySold = 120 });
            TopProducts.Add(new TopSellingProduct { ProductName = "Sạc Anker 65W (Sample)", TotalQuantitySold = 95 });

            // Dữ liệu giả cho biểu đồ
            RevenueLabels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
            RevenueSeries.Add(new ColumnSeries
            {
                Title = "Doanh thu",
                Values = new ChartValues<decimal> { 120000, 180000, 210000, 150000, 250000, 300000, 280000 }
            });
        }

        // --- Methods ---

        /// <summary>
        /// Phương thức chính để tải tất cả dữ liệu cho Dashboard.
        /// </summary>
        private void LoadDashboardData()
        {
            try
            {
                LoadKpiData();
                LoadRevenueData();
                LoadTopProductsData();
            }
            catch (Exception ex)
            {                
                Console.WriteLine($"Lỗi tải dữ liệu Dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// Tải dữ liệu cho các ô chỉ số KPI.
        /// </summary>
        private void LoadKpiData()
        {
            var kpisData = _dbService.GetDashboardKpis();
            KpiItems.Clear(); 

            if (kpisData != null)
            {
                KpiItems.Add(new KpiItem { Title = "Total Revenue", Value = kpisData.TotalRevenue.ToString("N0") + " VNĐ", Icon = "" });
                KpiItems.Add(new KpiItem { Title = "This Month's Revenue", Value = kpisData.MonthlyRevenue.ToString("N0") + " VNĐ", Icon = "" });
                KpiItems.Add(new KpiItem { Title = "Total Order", Value = kpisData.TotalSales.ToString(), Icon = "" });
                KpiItems.Add(new KpiItem { Title = "Total Customers", Value = kpisData.TotalCustomers.ToString(), Icon = "" });
            }
        }

        /// <summary>
        /// Tải và chuẩn bị dữ liệu doanh thu 7 ngày cho biểu đồ.
        /// </summary>
        private void LoadRevenueData()
        {
            var revenueData = _dbService.GetRevenueLast7Days();
            RevenueSeries.Clear(); 

            if (revenueData != null && revenueData.Any())
            {                
                RevenueLabels = revenueData.Select(d => d.SaleDate.ToString("dd/MM")).ToArray();
                OnPropertyChanged(nameof(RevenueLabels)); 
                
                RevenueSeries.Add(new ColumnSeries
                {
                    Title = "Doanh thu",
                    Values = new ChartValues<decimal>(revenueData.Select(d => d.Revenue)),
                    DataLabels = true, 
                    LabelPoint = point => point.Y.ToString("N0") 
                });
            }
        }

        /// <summary>
        /// Tải dữ liệu cho danh sách top sản phẩm bán chạy.
        /// </summary>
        private void LoadTopProductsData()
        {
            var topProductsData = _dbService.GetTopSellingProducts();
            TopProducts.Clear();

            if (topProductsData != null)
            {
                foreach (var item in topProductsData)
                {
                    TopProducts.Add(item);
                }
            }
        }

        // --- INotifyPropertyChanged Implementation ---
        // Cơ chế này giúp giao diện (View) tự động cập nhật khi dữ liệu trong ViewModel thay đổi.
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
