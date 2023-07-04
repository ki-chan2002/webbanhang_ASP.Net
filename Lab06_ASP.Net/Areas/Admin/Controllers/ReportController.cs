using AspNetCore.Reporting;
using Lab06_ASP.Net.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab06_ASP.Net.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ReportController : Controller
    {
        private readonly MyDBContext _dbContext;

        public ReportController(MyDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PrintCoffee()
        {
            using (var context = _dbContext)
            {
                var coffees = context.Coffees.ToList();
                var reportPath = "D:\\School\\File Visual Studio\\Lab06_ASP.Net\\ReportsApplication1\\Coffee Report.rdlc"; // Đường dẫn tới file template báo cáo RDLC

                var localReport = new LocalReport(reportPath);
                localReport.AddDataSource("DataSet1", coffees); // DataSet1 là tên dataset được định nghĩa trong file RDLC

                var result = localReport.Execute(RenderType.Pdf, 1);

                return File(result.MainStream, "application/pdf");
            }
        }
        public IActionResult PrintOrder()
        {
            using (var context = _dbContext)
            {
                var orders = context.Orders.ToList();
                var reportPath = "D:\\School\\File Visual Studio\\Lab06_ASP.Net\\ReportsApplication1\\Order Report.rdlc"; // Đường dẫn tới file template báo cáo RDLC

                var localReport = new LocalReport(reportPath);
                localReport.AddDataSource("DataSet2", orders); // DataSet1 là tên dataset được định nghĩa trong file RDLC

                var result = localReport.Execute(RenderType.Pdf, 1);

                return File(result.MainStream, "application/pdf");
            }
        }
    }
}
