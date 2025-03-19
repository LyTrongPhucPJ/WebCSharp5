using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GiaoDienAdmin.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        public DateTime? PaymentDate { get; set; }

        public byte PaymentStatus { get; set; }

        public int PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        [JsonIgnore]
        public PaymentMethod? PaymentMethod { get; set; }
    }
}
