using System;

namespace samp_01.Domain.Entities
{
     public class SellerProject
     {
         public int Id { get; set; }
         public int SellerId { get; set; }
         public string Title { get; set; } = null!;
         public string? Description { get; set; }
         public string? ImagePath { get; set; }
         public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
     }
}
