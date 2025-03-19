using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GiaoDienAdmin.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public bool Gender { get; set; } = true;

        public string? PassWord { get; set; } = string.Empty;


        public DateTime? DateOfBirth { get; set; }

        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Address { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        [JsonIgnore]
        public ICollection<Product>? Products { get; set; }
    }
}
