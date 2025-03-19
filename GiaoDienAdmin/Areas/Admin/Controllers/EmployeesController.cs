using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace GiaoDienAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employeeService;

        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

      






        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return View(employees);
        }

        // GET: Admin/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm nhân viên qua service
        [HttpPost("create")]
        public async Task<IActionResult> Create(string fullName, string phoneNumber, string email, DateTime? dateOfBirth, string address, bool gender, int roleId)
        {
            var result = await _employeeService.AddEmployeeAsync(fullName, phoneNumber, email, dateOfBirth, address, gender, roleId);

            if (result == "Thêm nhân viên thành công.")
            {
                TempData["SuccessMessage"] = result;
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = result;
            return View();
        }

        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            var existingEmployee = await _employeeService.GetEmployeeByIdAsync(id);
            if (existingEmployee == null) return NotFound();

            // Nếu mật khẩu không thay đổi, giữ mật khẩu cũ
            if (string.IsNullOrEmpty(employee.PassWord))
            {
                employee.PassWord = existingEmployee.PassWord;
            }

            if (ModelState.IsValid)
            {
                var result = await _employeeService.UpdateEmployeeAsync(id, employee);
                if (result)
                {
                    return RedirectToAction(nameof(Index));  // Nếu cập nhật thành công, chuyển hướng đến danh sách
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật nhân viên.");
                }
            }

            return View(employee);  // Nếu có lỗi hoặc model không hợp lệ, quay lại form và hiển thị lỗi
        }


        // GET: Admin/ProductCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Admin/ProductCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmloyeeAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
