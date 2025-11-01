using System;
using System.Collections.Generic;

namespace samp_01.Domain.DTO
{
    public class SellerPortfolioDTO
    {
        public int SellerId { get; set; }
        public string? Description { get; set; }
        public decimal? PriceRangeMin { get; set; }
        public decimal? PriceRangeMax { get; set; }
        public string? Skills { get; set; }
        public string? ProfilePicPath { get; set; }
    }

    public class SellerProjectDTO
    {
        public int Id { get; set; }
        public int SellerId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
