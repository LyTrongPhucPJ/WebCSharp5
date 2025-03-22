using GiaoDienAdmin.Helpers;
using GiaoDienAdmin.Models;
using GiaoDienAdmin.Services;
using GiaoDienAdmin.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GiaoDienAdmin.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        // Thanh toán từ giỏ hàng
        public IActionResult Checkout()
        {
            // Lấy dữ liệu từ session hoặc database
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            if (cartItems == null || !cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống.";
                return RedirectToAction("Index", "Cart");
            }

            // Tính tổng tiền
            var totalAmount = cartItems.Sum(item => item.Price * item.Quantity);

            // Tạo model cho VNPay
            var vnPayModel = new VnPaymentRequestModel
            {
                Amount = (double)totalAmount,
                CreatedDate = DateTime.Now,
                Description = "Thanh toán đơn hàng từ giỏ hàng",
                FullName = User.Identity.Name, // Tên người dùng hiện tại
                OrderId = new Random().Next(1000, 100000) // Mã đơn hàng ngẫu nhiên
            };

            // Chuyển hướng đến VNPay
            var paymentUrl = _vnPayService.CreatePaymentUrl(HttpContext, vnPayModel);
            return Redirect(paymentUrl);
        }

        // Xử lý callback từ VNPay
        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode ?? "Unknown"}";
                return RedirectToAction("PaymentFail");
            }

            TempData["Message"] = "Thanh toán VNPay thành công!";
            return RedirectToAction("PaymentSuccess");
        }

        // Trang thanh toán thành công
        public IActionResult PaymentSuccess()
        {
            ViewBag.Message = TempData["Message"] ?? "Giao dịch thành công.";
            return View();
        }

        // Trang thanh toán thất bại
        public IActionResult PaymentFail()
        {
            ViewBag.Message = TempData["Message"] ?? "Giao dịch thất bại.";
            return View();
        }

        public IActionResult Address()
        {
            return View();
        }

        public IActionResult Pay()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
