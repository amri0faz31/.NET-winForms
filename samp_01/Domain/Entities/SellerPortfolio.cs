using System;

namespace samp_01.Domain.Entities
{
     public class SellerPortfolio
     {
         public int Id { get; set; }
         public int SellerId { get; set; }
         public string? Description { get; set; }
         public decimal? PriceRangeMin { get; set; }
         public decimal? PriceRangeMax { get; set; }
         public string? Skills { get; set; }
         public string? ProfilePicPath { get; set; }
         public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
     }
}
