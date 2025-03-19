using GiaoDienAdmin.Models;
using Newtonsoft.Json;
using System.Text;

namespace GiaoDienAdmin.Services
{
    public class ProductCustomersService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7195/api/ProductsAdmin";
        private readonly string _baseUrl = "https://localhost:7195/api/ProductCategories";

        public ProductCustomersService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // ✅ Lấy danh sách danh mục sản phẩm
        public async Task<List<ProductCategories>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetStringAsync(_baseUrl);
            return JsonConvert.DeserializeObject<List<ProductCategories>>(response) ?? new List<ProductCategories>();
        }

        // ✅ Lấy tất cả sản phẩm và gán danh mục
        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetStringAsync(_apiUrl);
            var products = JsonConvert.DeserializeObject<List<Product>>(response) ?? new List<Product>();

            var categories = await GetCategoriesAsync();

            // Gán danh mục cho từng sản phẩm
            foreach (var product in products)
            {
                product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return products;
        }

        // ✅ Lấy sản phẩm theo ID
        public async Task<Product> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            var product = JsonConvert.DeserializeObject<Product>(response);

            if (product != null)
            {
                var categories = await GetCategoriesAsync();
                product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return product;
        }

        // ✅ Lấy sản phẩm theo danh mục
        public async Task<List<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/ProductCategories/Filter/{categoryId}");

            if (!response.IsSuccessStatusCode)
            {
                return new List<Product>(); // Trả về danh sách trống nếu lỗi
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Product>>(jsonString);
        }
    }
}
