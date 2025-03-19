using GiaoDienAdmin.Models;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace GiaoDienAdmin.Services
{
    public class CustomerService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7195/api/Customers"; // API URL

        // ✅ Constructor nhận HttpClient từ Dependency Injection
        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> GetRoleByPhoneNumberAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/get-customer-role/{phoneNumber}");

            if (!response.IsSuccessStatusCode)
            {
                return null; // 🔥 Nếu không tìm thấy, trả về null
            }

            return "Customer"; // 🔥 Nếu tìm thấy khách hàng, trả về "Customer"
        }




        // Lấy danh sách khách hàng từ API
        public async Task<List<Customer>> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync(_apiUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Customer>(); // 🟢 Trả về danh sách rỗng thay vì báo lỗi
            }

            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Customer>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Lấy khách hàng theo ID từ API
        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonSerializer.Deserialize<Customer>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Thêm khách hàng vào API
        public async Task<bool> CreateCustomerAsync(Customer customer)
        {
            var json = JsonSerializer.Serialize(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_apiUrl, content);
            return response.IsSuccessStatusCode;
        }

        // Cập nhật khách hàng trên API
        public async Task<bool> UpdateCustomerAsync(int id, Customer customer)
        {
            var json = JsonSerializer.Serialize(customer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        // Xóa khách hàng trên API
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        // Upload ảnh khách hàng lên API (nếu có hình ảnh)
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

        //Luân /////////////////////////////////////////////////////////////////////////////////////////
        public async Task<string> RegisterCustomerAsync(string phoneNumber, string email, string password, DateTime? dateOfBirth, string fullName, string address)
        {
            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return "Vui lòng nhập đầy đủ thông tin.";
            }

            // Kiểm tra số điện thoại đã tồn tại
            if (await CheckPhoneExistsAsync(phoneNumber))
            {
                return "Số điện thoại đã tồn tại.";
            }

            // Kiểm tra email đã tồn tại
            if (await CheckEmailExistsAsync(email))
            {
                return "Email đã tồn tại.";
            }

            // 🔥 Băm mật khẩu trước khi lưu
            string hashedPassword = HashHelper.ComputeSha256Hash(password);

            string finalAddress = string.IsNullOrEmpty(address) ? "Chưa có địa chỉ" : address;
            DateTime finalDateOfBirth = dateOfBirth ?? DateTime.Now;

            // Tạo đối tượng khách hàng mới
            var newCustomer = new Customer
            {
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Email = email,
                PassWord = hashedPassword,
                DateOfBirth = finalDateOfBirth,
                Address = finalAddress,
                IsActive = true
            };

            // Gửi yêu cầu POST đến API để thêm khách hàng
            var json = JsonSerializer.Serialize(newCustomer);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                return "Đăng ký thành công.";
            }
            return "Đăng ký thất bại, vui lòng thử lại.";
        }


        public async Task<Customer> AuthenticateCustomerAsync(string phoneNumber, string password)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/GetByPhoneNumber/{phoneNumber}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<Customer>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (customer != null && customer.PassWord.Trim() == HashHelper.ComputeSha256Hash(password))
                {
                    return customer;
                }
            }

            return null; // Không tìm thấy hoặc sai mật khẩu
        }




        // Lấy mật khẩu khách hàng theo số điện thoại từ API
        public async Task<string> GetCustomerPasswordByPhoneNumber(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/GetByPhoneNumber/{phoneNumber}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<Customer>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return customer?.PassWord;  // Trả về mật khẩu nếu có
            }

            return null;  // Nếu không tìm thấy khách hàng, trả về null
        }


        // Kiểm tra Email đã tồn tại
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/check-email?email={email}");
            return response.IsSuccessStatusCode && bool.Parse(await response.Content.ReadAsStringAsync());
        }

        // Kiểm tra SĐT đã tồn tại
        public async Task<bool> CheckPhoneExistsAsync(string phone)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/check-phone?phone={phone}");
            return response.IsSuccessStatusCode && bool.Parse(await response.Content.ReadAsStringAsync());
        }

        // Lấy thông tin khách hàng theo số điện thoại
        public async Task<Customer> GetCustomerByPhoneNumber(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/GetByPhoneNumber/{phoneNumber}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // Return null if no customer is found
            }

            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Customer>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        //Lấy email từ SĐT
        public async Task<string> GetCustomerEmailByPhoneNumberAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7195/api/Customers/GetByPhoneNumber/{phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
                return customer?.Email ?? "Không tìm thấy email";
            }
            return "Không tìm thấy email";
        }

        // DTO ánh xạ dữ liệu từ API
        public class CustomerDto
        {
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }
    }
}
