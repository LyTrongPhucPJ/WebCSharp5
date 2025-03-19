using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace GiaoDienAdmin.Controllers
{


    public class UserController : Controller
    {
        private readonly CustomerService _customerService;
        private readonly EmployeeService _employeeService;  // Add EmployeeService

        public UserController(CustomerService customerService, EmployeeService employeeService)
        {
            _customerService = customerService;
            _employeeService = employeeService;  // Inject EmployeeService
        }
















        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string phoneNumber, string email, string password, DateTime? dateOfBirth, string fullName, string address)
        {
            string result = await _customerService.RegisterCustomerAsync(phoneNumber, email, password, dateOfBirth, fullName, address);

            if (result != "Đăng ký thành công.")
            {
                ModelState.AddModelError("", result);
                return View();
            }

            return RedirectToAction("LoginPhone");
        }




        [HttpGet]
        public IActionResult LoginPhone()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPhone(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                ModelState.AddModelError("", "Vui lòng nhập số điện thoại.");
                return View();
            }

            // Kiểm tra số điện thoại có tồn tại trong Employees hoặc Customers không
            var employee = await _employeeService.GetEmployeeByPhoneNumber(phoneNumber);
            var customer = await _customerService.GetCustomerByPhoneNumber(phoneNumber);

            if (employee != null || customer != null)
            {
                TempData["PhoneNumber"] = phoneNumber;  // Lưu số điện thoại tạm thời
                return RedirectToAction("LoginPassword");
            }

            ModelState.AddModelError("", "Số điện thoại chưa được đăng ký.");
            return View();
        }

        public IActionResult LoginPassword()
        {
            if (TempData["PhoneNumber"] == null)
            {
                return RedirectToAction("LoginPhone");
            }
            ViewBag.PhoneNumber = TempData["PhoneNumber"].ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPassword(string phoneNumber, string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập mật khẩu.");
                return View();
            }

            // 🔥 Băm mật khẩu nhập vào trước khi so sánh
            string hashedInputPassword = HashHelper.ComputeSha256Hash(password);

            var employee = await _employeeService.AuthenticateEmployeeAsync(phoneNumber, hashedInputPassword);
            if (employee != null)
            {
                Console.WriteLine($"✅ Nhân viên đăng nhập: {employee.FullName}, ID: {employee.Id}, Role: {employee.Role?.Name}");

                // Kiểm tra nếu tài khoản đăng nhập lần đầu (mật khẩu mặc định)
                string defaultHashedPassword = HashHelper.ComputeSha256Hash("123456");
                if (hashedInputPassword == defaultHashedPassword)
                {
                    Console.WriteLine("🔑 Mật khẩu mặc định, yêu cầu đổi mật khẩu!");
                    return RedirectToAction("ChangePassword", "Employees", new { id = employee.Id });
                }

                return await SignInWithPhoneNumber(employee.Id, employee.FullName, phoneNumber, employee.Role?.Name);
            }

            // Kiểm tra đăng nhập với Customer
            var customer = await _customerService.AuthenticateCustomerAsync(phoneNumber, password);
            if (customer != null)
            {
                return SignInWithPhoneNumber1(customer.Id, customer.FullName, phoneNumber, "Customer");
            }




            Console.WriteLine("❌ Sai mật khẩu hoặc tài khoản không tồn tại!");
            ModelState.AddModelError("", "Mật khẩu không chính xác.");
            ViewBag.PhoneNumber = phoneNumber;
            return View();
        }

        private IActionResult SignInWithPhoneNumber1(int userId, string fullName, string phoneNumber, string role)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, fullName),
        new Claim("PhoneNumber", phoneNumber),  // Lưu số điện thoại vào Claims
        new Claim(ClaimTypes.Role, role)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập người dùng vào hệ thống bằng Cookie Authentication
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Chuyển hướng người dùng đến trang chủ
            return RedirectToAction("Index", "ProductCustomers");
        }




        private async Task<IActionResult> SignInWithPhoneNumber(int userId, string fullName, string phoneNumber, string roleName)
        {
            Console.WriteLine($"🔍 Kiểm tra vai trò - SĐT: {phoneNumber}, Role yêu cầu: {roleName}");

            if (string.IsNullOrEmpty(roleName))
            {
                // 🔹 Nếu vai trò NULL, kiểm tra trong bảng Nhân viên
                roleName = await _employeeService.GetRoleByPhoneNumberAsync(phoneNumber);

              
            }


            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, fullName),
        new Claim("PhoneNumber", phoneNumber),
        new Claim(ClaimTypes.Role, roleName ?? "Unknown")
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return roleName switch
            {
                "Nhân Viên" => RedirectToAction("Index", "Staffs"),
              
                _ => RedirectToAction("Index", "Home")
            };
        }




        private IActionResult SignInUser(int userId, string fullName, string role)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, fullName),
        new Claim(ClaimTypes.Role, role)
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Đăng nhập người dùng vào hệ thống bằng Cookie Authentication
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Chuyển hướng người dùng đến trang chủ
            return RedirectToAction("Index", "ProductCustomers");
        }




        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
            Console.WriteLine($"Phone Number from Claims: {phoneNumber}"); // Debug

            string userEmail = "Không tìm thấy email";

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                // Lấy email của khách hàng
                userEmail = await _customerService.GetCustomerEmailByPhoneNumberAsync(phoneNumber);
                Console.WriteLine($"Fetched Email from Customer API: {userEmail}"); // Debug

                // Nếu không có email khách hàng, kiểm tra nhân viên
                if (userEmail == "Không tìm thấy email")
                {
                    userEmail = await _employeeService.GetEmployeeEmailByPhoneNumberAsync(phoneNumber);
                    Console.WriteLine($"Fetched Email from Employee API: {userEmail}"); // Debug
                }
            }

            ViewBag.UserName = User.Identity.Name;
            ViewBag.UserEmail = userEmail;

            return View();
        }




        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                Console.WriteLine("📢 Đang đăng xuất người dùng...");

                // Kiểm tra nếu Session có được cấu hình không
                if (HttpContext.Session == null)
                {
                    Console.WriteLine("❌ Lỗi: Session chưa được cấu hình.");
                    return StatusCode(500, "Session chưa được cấu hình.");
                }

                HttpContext.Session.Clear(); // Xóa session nếu có

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                Console.WriteLine("✅ Đăng xuất thành công! Chuyển về LoginPhone");

                return RedirectToAction("LoginPhone", "User");
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi đăng xuất: " + ex.Message);
                return StatusCode(500, "Lỗi server khi đăng xuất: " + ex.Message);
            }
        }





        [HttpGet]
        public IActionResult GetUserName()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name ?? "Người dùng";
                Console.WriteLine($"✅ Tên đăng nhập: {userName}");
                return Json(new { userName });
            }

            Console.WriteLine("❌ Người dùng chưa đăng nhập!");
            return Unauthorized();
        }

    }


}