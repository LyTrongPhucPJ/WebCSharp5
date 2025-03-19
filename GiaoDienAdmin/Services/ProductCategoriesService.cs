using GiaoDienAdmin.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GiaoDienAdmin.Services
{
    public class ProductCategoriesService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7195/api/ProductCategories"; // API URL

        // ✅ Constructor nhận HttpClient từ Dependency Injection
        public ProductCategoriesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Lấy danh sách danh mục từ API
        public async Task<List<ProductCategories>> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync(_apiUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<ProductCategories>(); // 🟢 Trả về danh sách rỗng thay vì báo lỗi
            }

            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ProductCategories>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Lấy danh mục theo ID từ API
        public async Task<ProductCategories> GetCategoryByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonSerializer.Deserialize<ProductCategories>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Thêm danh mục vào API
        public async Task<bool> CreateCategoryAsync(ProductCategories category)
        {
            var json = JsonSerializer.Serialize(category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật danh mục trên API
        public async Task<bool> UpdateCategoryAsync(int id, ProductCategories category)
        {
            var json = JsonSerializer.Serialize(category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // Xóa danh mục trên API
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Upload ảnh lên API (nếu có hình ảnh cho danh mục)
        public async Task<string> UploadImageToApiAsync(IFormFile file)
        {
            using var formData = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            formData.Add(fileContent, "imageFile", file.FileName);

            var response = await _httpClient.PostAsync($"{_apiUrl}/upload", formData);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }

        // Xóa ảnh khỏi API
        public async Task<bool> DeleteImageFromApiAsync(string imagePath)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/delete-image?path={imagePath}");
            return response.IsSuccessStatusCode;
        }
    }
}
