using System.ComponentModel.DataAnnotations;

namespace GiaoDienAdmin.Areas.Admin.Models
{
    public class Employees
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống"), MaxLength(67)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống"), MaxLength(12)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [MaxLength(80, ErrorMessage = "Mật khẩu không được vượt quá 80 ký tự")]
        public string PassWord { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống"), EmailAddress(ErrorMessage = "Email không hợp lệ"), MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
