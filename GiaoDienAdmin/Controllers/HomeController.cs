using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GiaoDienAdmin.Controllers
{
    public class HomeController : Controller
    {
        /*  private readonly ILogger<HomeController> _logger;

          public HomeController(ILogger<HomeController> logger)
          {
              _logger = logger;
          }

          public IActionResult Index()
          {
              return View();
          }

          public IActionResult Privacy()
          {
              return View();
          }

          [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
          public IActionResult Error()
          {
              return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
          }*/

        private readonly ProductService _productService;
        private readonly CustomerService _customerService;

        public HomeController(ProductService productService, CustomerService customerService)
        {
            _productService = productService;
            _customerService = customerService;
        }

        // GET: Trang Chủ
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách sản phẩm từ ProductKHService
            var products = await _productService.GetProductsAsync();

            // Lấy danh sách khách hàng từ CustomerService (nếu cần thiết)
            var customers = await _customerService.GetCustomersAsync();

            // Trả về view và truyền danh sách sản phẩm (có thể có khách hàng nếu cần)
            return View((products, customers));  // Truyền tuple (products, customers) vào view
        }

    }
}
