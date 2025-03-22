using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace GiaoDienAdmin.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CustomersController : Controller
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetCustomersAsync();
            return View(customers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id.Value);
            if (customer == null) return NotFound();

            return View(customer);
        }  

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (customer == null)
                {
                    ModelState.AddModelError("", "Dữ liệu khách hàng không hợp lệ.");
                    return View(customer);
                }

                var result = await _customerService.CreateCustomerAsync(customer);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi tạo khách hàng.");
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id.Value);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id) return NotFound();

            if (ModelState.IsValid)
            {
                customer.IsActive = customer.IsActive;

                var result = await _customerService.UpdateCustomerAsync(id, customer);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi khi cập nhật khách hàng.");
                }
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var customer = await _customerService.GetCustomerByIdAsync(id.Value);
            if (customer == null) return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _customerService.DeleteCustomerAsync(id);
            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Lỗi khi xóa khách hàng.");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
