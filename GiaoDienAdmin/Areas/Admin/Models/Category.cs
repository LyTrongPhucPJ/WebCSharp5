using GiaoDienAdmin.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GiaoDienAdmin.Areas.Admin.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "nvarchar(25)")]
        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        [StringLength(25, ErrorMessage = "Tên danh mục không được vượt quá 25 ký tự.")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
