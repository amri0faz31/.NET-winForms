using System;

namespace samp_01.Domain.Entities
{
     public class Payment
     {
         public int Id { get; set; }
         public int OrderId { get; set; }
         public decimal Amount { get; set; }
         public string Status { get; set; } = "Pending"; // Pending, Paid, Refunded
         public DateTime? PaidAt { get; set; }
     }
}
