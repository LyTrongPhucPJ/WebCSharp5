using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class EmployeesController : Controller
{
    private readonly EmployeeService _employeeService;

    public EmployeesController(EmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // ✅ Hiển thị form đổi mật khẩu
    [HttpGet]
    public IActionResult ChangePassword()
    {
      
        return View();
    }

    // ✅ Xử lý đổi mật khẩu
    [HttpPost]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
        {
            ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
            return View();
        }

        if (newPassword.Length < 6)
        {
            ModelState.AddModelError("", "Mật khẩu mới phải có ít nhất 6 ký tự.");
            return View();
        }

        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
            return View();
        }

        var phoneNumber = User.FindFirst("PhoneNumber")?.Value;
        if (string.IsNullOrEmpty(phoneNumber))
        {
            return Unauthorized();
        }

        // ✅ Xác định ID nhân viên dựa trên số điện thoại
        var employee = await _employeeService.GetEmployeeByPhoneNumber(phoneNumber);
        if (employee == null)
        {
            return Unauthorized("Không tìm thấy tài khoản.");
        }

        string result = await _employeeService.ChangePasswordAsync(employee.Id, oldPassword, newPassword);

        if (result == "Đổi mật khẩu thành công.")
        {
            TempData["SuccessMessage"] = result;
            return RedirectToAction("Profile");
        }

        ModelState.AddModelError("", result);
        return View();
    }
}
