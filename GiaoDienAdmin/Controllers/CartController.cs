using System;
using GiaoDienAdmin.Areas.Admin.Data;
using GiaoDienAdmin.Helpers;
using GiaoDienAdmin.Models;
using GiaoDienAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GiaoDienAdmin.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        public CartController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var cartVM = new CartItemViewModel
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };

            return View(cartVM);
        }

        public IActionResult AddToCart()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            var cartVM = new CartItemViewModel
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };

            return View(cartVM);
        }

        public async Task<IActionResult> Add(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["Error"] = "Sản phẩm không tồn tại.";
                return RedirectToAction("Index");
            }

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == id).FirstOrDefault();

            if (cartItem == null)
            {
                // Nếu sản phẩm chưa có trong giỏ hàng, thêm sản phẩm mới
                cart.Add(new CartItemModel
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = 1,
                    Image = product.Img // Đảm bảo `product.Image` có giá trị
                });
            }
            else
            {
                // Nếu sản phẩm đã tồn tại, tăng số lượng
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);
            TempData["Success"] = $"Sản phẩm \"{product.Name}\" đã được thêm vào giỏ hàng!";
            var referer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(referer))
            {
                return RedirectToAction("Index");
            }
            return Redirect(referer);
        }

        public IActionResult IncreaseQuantity(int id)
        {
            // Lấy giỏ hàng từ Session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm sản phẩm trong giỏ hàng
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity += 1; // Tăng số lượng sản phẩm
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index");
        }

        public IActionResult DecreaseQuantity(int id)
        {
            // Lấy giỏ hàng từ Session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm sản phẩm trong giỏ hàng
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cartItem.Quantity -= 1; // Giảm số lượng sản phẩm

                // Nếu số lượng <= 0, xóa sản phẩm khỏi giỏ hàng
                if (cartItem.Quantity <= 0)
                {
                    cart.Remove(cartItem);
                }
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index");
        }
        public IActionResult Remove(int id)
        {
            // Lấy giỏ hàng từ Session
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm sản phẩm trong giỏ hàng
            var cartItem = cart.FirstOrDefault(c => c.ProductId == id);

            if (cartItem != null)
            {
                cart.Remove(cartItem); // Xóa sản phẩm khỏi giỏ hàng
            }

            // Lưu lại giỏ hàng vào Session
            HttpContext.Session.SetJson("Cart", cart);

            TempData["Success"] = "Sản phẩm đã được xóa khỏi giỏ hàng!";
            return RedirectToAction("Index");
        }

    }
}
