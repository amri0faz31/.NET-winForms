using System;

namespace samp_01.Domain.Entities
{
     public class Message
     {
         public int Id { get; set; }
         public int? OrderId { get; set; } // null for direct messages
         public int SenderId { get; set; }
         public string SenderType { get; set; } = null!; // "User" or "Seller"
         public int? ReceiverId { get; set; } // for direct messages
         public string? ReceiverType { get; set; } // "User" or "Seller" for direct messages
         public string? Body { get; set; }
         public string? FilePath { get; set; }
         public DateTime SentAt { get; set; } = DateTime.UtcNow;
     }
}
