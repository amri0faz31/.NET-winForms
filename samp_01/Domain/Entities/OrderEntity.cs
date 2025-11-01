using System;

namespace samp_01.Domain.Entities
{
     public class Order
     {
             public int Id { get; set; }
             public int UserId { get; set; }
             public int SellerId { get; set; }
             public int ServiceId { get; set; }
             public string Title { get; set; } = null!;
             public string? Requirements { get; set; }
             public decimal TotalPrice { get; set; }
             public string Status { get; set; } = "Pending";
             public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
             public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
     }
}
