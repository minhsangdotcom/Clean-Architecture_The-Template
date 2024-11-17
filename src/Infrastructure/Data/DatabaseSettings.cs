using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Data;

public class DatabaseSettings
{
    [Required]
    public string? DatabaseConnection { get; set; }
}
