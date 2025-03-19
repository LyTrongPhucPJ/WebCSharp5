using GiaoDienAdmin.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Role
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }  // Có thể null

    // Navigation Property
    [JsonIgnore]
    public ICollection<Employee>? Employees { get; set; }
}
