using System;

namespace samp_01.Domain.DTO
{
    public class AdminProfileDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
