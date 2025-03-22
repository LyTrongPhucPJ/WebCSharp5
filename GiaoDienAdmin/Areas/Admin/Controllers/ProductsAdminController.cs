using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using Microsoft.AspNetCore.Mvc;

namespace GiaoDienAdmin.Controllers
{
    [Area("Admin")]
    public class ProductsAdminController : Controller
    {
        private readonly ProductService _productService;



        public ProductsAdminController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var products = await _productService.GetProductsAsync();

                return View(products);
            }
            catch (Exception ex)
            {
               
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải dữ liệu sản phẩm.";
                return View(new List<Product>());
            }
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _productService.GetCategoriesAsync();
            ViewBag.Employee = await _productService.GetEmployeeAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                ViewBag.Employee = await _productService.GetEmployeeAsync();
                return View(product);
            }

            bool success = await _productService.CreateProductAsync(product, img);  

            if (!success)
            {
                ModelState.AddModelError("", "Không thể thêm sản phẩm. Kiểm tra lại API!");
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                ViewBag.Employee = await _productService.GetEmployeeAsync();
                return View(product);
            }

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = await _productService.GetCategoriesAsync();
            ViewBag.Employee = await _productService.GetEmployeeAsync();
            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                ViewBag.Employee = await _productService.GetEmployeeAsync();
                return View(product);
            }

            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = string.IsNullOrEmpty(product.Name) ? existingProduct.Name : product.Name;
            existingProduct.Price = product.Price <= 0 ? existingProduct.Price : product.Price;
            existingProduct.Description = string.IsNullOrEmpty(product.Description) ? existingProduct.Description : product.Description;
            existingProduct.IsActive = product.IsActive;
            existingProduct.CategoryId = product.CategoryId != 0 ? product.CategoryId : existingProduct.CategoryId;
            existingProduct.EmployeeId = product.EmployeeId != 0 ? product.EmployeeId : existingProduct.EmployeeId;

            if (ImageFile != null)
            {
                string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string filePath = Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }
                existingProduct.Img = "/images/" + fileName;
            }

            bool success = await _productService.UpdateProductAsync(id, existingProduct, ImageFile);

            if (!success)
            {
                ModelState.AddModelError("", "Không thể cập nhật sản phẩm. Kiểm tra lại API!");
                ViewBag.Categories = await _productService.GetCategoriesAsync();
                ViewBag.Employee = await _productService.GetEmployeeAsync();
                return View(existingProduct);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImageAsync(IFormFile ImageFile)
        {
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ImageFile.CopyToAsync(stream);
            }

            return $"/images/{fileName}";
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product); 
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _productService.DeleteProductAsync(id);

            if (success)
            {
                TempData["SuccessMessage"] = "Xóa sản phẩm thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa sản phẩm!";
            }

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product); 
        }

    }
}