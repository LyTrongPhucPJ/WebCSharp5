using GiaoDienAdmin.Models;
using System.Text.Json;
using System.Text;

namespace GiaoDienAdmin.Services
{
    public class EmployeeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://localhost:7195/api/Employees"; // API URL

        // Constructor nhận HttpClient từ Dependency Injection
        public EmployeeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<string> ChangePasswordAsync(int employeeId, string oldPassword, string newPassword)
        {
            var request = new
            {
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // 🔥 Gọi API đổi mật khẩu
            var response = await _httpClient.PutAsync($"https://localhost:7195/api/Employees/change-password/{employeeId}", content);

            if (response.IsSuccessStatusCode)
            {
                return "Đổi mật khẩu thành công.";
            }

            var errorResponse = await response.Content.ReadAsStringAsync();
            return $"Đổi mật khẩu thất bại: {errorResponse}";
        }









      



        public async Task<string?> GetRoleByPhoneNumberAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/get-role/{phoneNumber}");

            if (!response.IsSuccessStatusCode)
            {
                return null; // Không tìm thấy số điện thoại
            }

            var role = await response.Content.ReadAsStringAsync();
            return role;
        }





        // Lấy danh sách nhân viên từ API
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var response = await _httpClient.GetAsync(_apiUrl);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new List<Employee>(); // Trả về danh sách rỗng nếu không có nhân viên nào
            }

            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Employee>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Lấy nhân viên theo ID từ API
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            var response = await _httpClient.GetStringAsync($"{_apiUrl}/{id}");
            return JsonSerializer.Deserialize<Employee>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        //// Thêm nhân viên vào API
        //public async Task<bool> CreateEmployeeAsync(Employee employee)
        //{
        //    var json = JsonSerializer.Serialize(employee);
        //    var content = new StringContent(json, Encoding.UTF8, "application/json");

        //    var response = await _httpClient.PostAsync("https://localhost:7195/api/Employees", content);  // Đảm bảo URL chính xác
        //    return response.IsSuccessStatusCode;
        //}


        public async Task<string> AddEmployeeAsync(string fullName, string phoneNumber, string email, DateTime? dateOfBirth, string address, bool gender, int roleId)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(email))
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

            // Mật khẩu mặc định
            string defaultPassword = "123456";

            // 🔥 Băm mật khẩu bằng SHA-256 + Base64
            string hashedPassword = HashHelper.ComputeSha256Hash(defaultPassword);

            // Định dạng ngày sinh và địa chỉ mặc định
            DateTime finalDateOfBirth = dateOfBirth ?? DateTime.Now;
            string finalAddress = string.IsNullOrEmpty(address) ? "Chưa có địa chỉ" : address;

            // Tạo đối tượng nhân viên mới
            var newEmployee = new Employee
            {
                FullName = fullName,
                PhoneNumber = phoneNumber,
                Email = email,
                PassWord = hashedPassword,
                DateOfBirth = finalDateOfBirth,
                Address = finalAddress,
                Gender = gender,
                RoleId = roleId,
                IsActive = true
            };

            // Gửi yêu cầu POST đến API để thêm nhân viên
            var json = JsonSerializer.Serialize(newEmployee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, content);
            if (response.IsSuccessStatusCode)
            {
                return "Thêm nhân viên thành công.";
            }
            return "Thêm nhân viên thất bại, vui lòng thử lại.";
        }


        private async Task<bool> CheckPhoneExistsAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/check-phone/{phoneNumber}");
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/check-email/{email}");
            return response.IsSuccessStatusCode;
        }


        // Cập nhật nhân viên trên API
        public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
        {
            var json = JsonSerializer.Serialize(employee);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_apiUrl}/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteEmloyeeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_apiUrl}/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<Employee> GetEmployeeByPhoneNumber(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/GetByPhoneNumber/{phoneNumber}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null; // Trả về null nếu không tìm thấy nhân viên
            }

            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Employee>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Employee> AuthenticateEmployeeAsync(string phoneNumber, string password)
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/GetByPhoneNumber/{phoneNumber}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var employee = JsonSerializer.Deserialize<Employee>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (employee != null && employee.PassWord.Trim() == HashHelper.ComputeSha256Hash(password))
                {
                    return employee;
                }
            }

            return null; // Không tìm thấy hoặc sai mật khẩu
        }


        public async Task<string> GetEmployeeEmailByPhoneNumberAsync(string phoneNumber)
        {
            var response = await _httpClient.GetAsync($"https://localhost:7195/api/Employees/GetByPhoneNumber/{phoneNumber}");
            if (response.IsSuccessStatusCode)
            {
                var employee = await response.Content.ReadFromJsonAsync<EmployeeDto>();
                return employee?.Email ?? "Không tìm thấy email";
            }
            return "Không tìm thấy email";
        }
        // DTO ánh xạ dữ liệu từ API (Nhân viên)
        public class EmployeeDto
        {
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

    }
}
