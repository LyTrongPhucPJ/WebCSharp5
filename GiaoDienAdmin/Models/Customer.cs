using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GiaoDienAdmin.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string? PassWord { get; set; } = string.Empty;


        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }
    }
}
