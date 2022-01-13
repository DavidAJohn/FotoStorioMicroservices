using System.ComponentModel.DataAnnotations;

namespace Discount.Grpc.Models;

public class Campaign : BaseEntity
{
    [Required]
    public string Name { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}
