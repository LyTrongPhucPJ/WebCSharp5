using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GiaoDienAdmin.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;// có

        [Column(TypeName = "decimal(18,3)")]
        public decimal Price { get; set; }// có

        public string? Img { get; set; }// có

        [NotMapped] // Không lưu vào database
        [JsonIgnore]
        public IFormFile? ImageFile { get; set; } // Nhận file ảnh từ form

        public int CategoryId { get; set; }// có

        public int EmployeeId { get; set; }// có

        public string? Description { get; set; }// có

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; //thêm ko có ngày tạo

        public bool IsActive { get; set; } = true;// có

        [ForeignKey("CategoryId")]
        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public ProductCategories? Category { get; set; }

        [ForeignKey("EmployeeId")]
        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public Employee? Employee { get; set; }

        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public ICollection<Feedback>? Feedbacks { get; set; }

        [JsonIgnore] // Bỏ qua khi tuần tự hóa JSON
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
