using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;

namespace GiaoDienAdmin.Controllers
{
    public class ProductCustomersController : Controller
    {
        private readonly ProductCustomersService _productCustomersService;

        public ProductCustomersController(ProductCustomersService productService)
        {
            _productCustomersService = productService;
        }

        [HttpGet("Filter/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productCustomersService.GetProductsAsync();

            var filteredProducts = products
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            if (!filteredProducts.Any())
            {
                return NotFound($"Không tìm thấy sản phẩm nào trong danh mục ID = {categoryId}");
            }

            return Ok(filteredProducts);
        }


        public async Task<IActionResult> Locsp()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }

        public async Task<IActionResult> Locsp1()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }

        // ✅ Hiển thị danh mục và sản phẩm
        public async Task<IActionResult> Index()
        {
            var categories = await _productCustomersService.GetCategoriesAsync();
            var products = await _productCustomersService.GetProductsAsync();

            ViewBag.Categories = categories; // Gửi danh mục qua ViewBag
            return View(products);
        }

        // ✅ Hiển thị chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            // Lấy chi tiết sản phẩm theo ID
            var product = await _productCustomersService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm.");
            }

            // Kiểm tra danh mục sản phẩm
            if (product.CategoryId == 0)
            {
                ViewBag.SuggestedProducts = new List<Product>(); // Tránh lỗi gọi API sai
            }
            else
            {
                // Lấy sản phẩm gợi ý theo danh mục
                var suggestedProducts = await _productCustomersService.GetProductsByCategoryAsync(product.CategoryId);

                // Lọc sản phẩm và loại trừ sản phẩm hiện tại
                var filteredSuggestedProducts = suggestedProducts.Where(p => p.Id != id).Take(4).ToList();

                // Kiểm tra có sản phẩm gợi ý không
                if (filteredSuggestedProducts.Count == 0)
                {
                    Console.WriteLine("Không có sản phẩm gợi ý.");
                }

                ViewBag.SuggestedProducts = filteredSuggestedProducts;
            }

            return View(product);
        }


        public async Task<IActionResult> LocspBanh()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }
        public async Task<IActionResult> LocspNuoc()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }

        public async Task<IActionResult> LocspTra()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }

        public async Task<IActionResult> LocspCaPhe()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }
        public async Task<IActionResult> Promotion()
        {
            var categories = await _productCustomersService.GetCategoriesAsync(); // Lấy danh mục sản phẩm
            var products = await _productCustomersService.GetProductsAsync(); // Lấy tất cả sản phẩm

            ViewBag.Categories = categories; // Truyền danh mục vào ViewBag
            ViewBag.SelectedCategory = ""; // Chưa chọn danh mục nào mặc định

            return View(products); // Trả về sản phẩm và truyền danh mục qua ViewBag
        }

    }
}
