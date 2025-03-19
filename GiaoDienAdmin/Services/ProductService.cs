
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using GiaoDienAdmin.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GiaoDienAdmin.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly string _productApiUrl = "https://localhost:7195/api/ProductsAdmin";
        private readonly string _categoryApiUrl = "https://localhost:7195/api/ProductCategories";
        private readonly string _employeeApiUrl = "https://localhost:7195/api/Employees";



        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await _httpClient.GetStringAsync(_productApiUrl);
            var products = JsonConvert.DeserializeObject<List<Product>>(response) ?? new List<Product>();

            // Lấy danh mục và nhân viên để gắn vào từng sản phẩm
            var categoryResponse = await _httpClient.GetStringAsync(_categoryApiUrl);
            var categories = JsonConvert.DeserializeObject<List<ProductCategories>>(categoryResponse) ?? new List<ProductCategories>();

            var employeeResponse = await _httpClient.GetStringAsync(_employeeApiUrl);
            var employees = JsonConvert.DeserializeObject<List<Employee>>(employeeResponse) ?? new List<Employee>();

            foreach (var product in products)
            {
                // Gán danh mục và nhân viên cho sản phẩm
                product.Category = categories.FirstOrDefault(c => c.Id == product.CategoryId);
                product.Employee = employees.FirstOrDefault(e => e.Id == product.EmployeeId);
            }

            return products;
        }


        public async Task<List<SelectListItem>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetStringAsync(_categoryApiUrl);
            var categories = JsonConvert.DeserializeObject<List<ProductCategories>>(response) ?? [];

            // Chuyển đổi danh mục thành SelectListItem
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
        }
        public async Task<List<SelectListItem>> GetEmployeeAsync()
        {
            var response = await _httpClient.GetStringAsync(_employeeApiUrl);
            var categories = JsonConvert.DeserializeObject<List<Employee>>(response) ?? [];

            // Chuyển đổi danh mục thành SelectListItem
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.FullName
            }).ToList();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_productApiUrl}/{id}");
            return JsonConvert.DeserializeObject<Product>(response);
        }

     

        public async Task<bool> CreateProductAsync(Product product, IFormFile img)
        {
            using var formData = new MultipartFormDataContent();

            // Kiểm tra và thêm ảnh vào form data
            if (img != null)
            {
                var fileStream = img.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(img.ContentType);
                formData.Add(fileContent, "image", img.FileName);  // Gửi ảnh từ client lên API
            }

            // Thêm các thông tin khác của sản phẩm vào form data
            formData.Add(new StringContent(product.Name), "Name");
            formData.Add(new StringContent(product.Price.ToString()), "Price");
            formData.Add(new StringContent(product.CategoryId.ToString()), "CategoryId");
            formData.Add(new StringContent(product.EmployeeId.ToString()), "EmployeeId");
            formData.Add(new StringContent(product.Description ?? ""), "Description");
            formData.Add(new StringContent(product.IsActive.ToString()), "IsActive");

            // Gửi yêu cầu POST tới API
            var response = await _httpClient.PostAsync(_productApiUrl, formData);
            return response.IsSuccessStatusCode;
        }





        public async Task<bool> UpdateProductAsync(int id, Product product, IFormFile? ImageFile)
        {
            using var formData = new MultipartFormDataContent();

            // Nếu có ảnh mới, thêm vào form data
            if (ImageFile != null)
            {
                var fileStream = ImageFile.OpenReadStream();
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(ImageFile.ContentType);
                formData.Add(fileContent, "images", ImageFile.FileName);  // Gửi ảnh lên API
            }

            // Thêm các thông tin khác của sản phẩm vào form data
            if (!string.IsNullOrEmpty(product.Name))
                formData.Add(new StringContent(product.Name), "Name");

            if (product.Price != 0)
                formData.Add(new StringContent(product.Price.ToString()), "Price");

            if (!string.IsNullOrEmpty(product.Description))
                formData.Add(new StringContent(product.Description), "Description");

            formData.Add(new StringContent(product.IsActive.ToString()), "IsActive");
            formData.Add(new StringContent(product.CategoryId.ToString()), "CategoryId");
            formData.Add(new StringContent(product.EmployeeId.ToString()), "EmployeeId");

            // Gửi yêu cầu PUT để cập nhật sản phẩm
            var response = await _httpClient.PutAsync($"{_productApiUrl}/{id}", formData);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> DeleteProductAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_productApiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }


        public async Task<List<Product>> GetSuggestedProductsAsync(int categoryId, int excludeProductId)
        {
            // Lấy tất cả sản phẩm từ API
            var response = await _httpClient.GetStringAsync(_productApiUrl);
            var products = JsonConvert.DeserializeObject<List<Product>>(response);

            // Lọc các sản phẩm thuộc cùng danh mục, loại bỏ sản phẩm hiện tại
            var suggestedProducts = products
                .Where(p => p.CategoryId == categoryId && p.Id != excludeProductId)
                .Take(4) // Giới hạn tối đa 4 sản phẩm
                .ToList();

            return suggestedProducts;
        }

    }
}