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

        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetEmployeesAsync();
            return View(employees);
        }

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

        /* [HttpPost("create")]
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
         }*/

        [HttpPost("create")]
        public async Task<IActionResult> Create(string fullName, string phoneNumber, string email, DateTime? dateOfBirth, string address, bool gender, int roleId)
        {
            // Gọi service để thêm nhân viên
            var result = await _employeeService.AddEmployeeAsync(fullName, phoneNumber, email, dateOfBirth, address, gender, roleId);

            // Nếu thêm thành công, chuyển hướng về Index
            if (result == "Thêm nhân viên thành công.")
            {
                TempData["SuccessMessage"] = result; // Lưu thông báo thành công để hiển thị ở trang Index
                return RedirectToAction("Index");
            }

            // Nếu có lỗi, hiển thị thông báo lỗi trên giao diện Create
            ViewBag.ErrorMessage = result;
            return View();
        }


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

            if (string.IsNullOrEmpty(employee.PassWord))
            {
                employee.PassWord = existingEmployee.PassWord;
            }

            if (ModelState.IsValid)
            {
                var result = await _employeeService.UpdateEmployeeAsync(id, employee);
                if (result)
                {
                    return RedirectToAction(nameof(Index));  
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật nhân viên.");
                }
            }

            return View(employee);  
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteEmloyeeAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
