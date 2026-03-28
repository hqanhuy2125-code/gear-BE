using System;
using System.ComponentModel.DataAnnotations;

namespace GamingGearBackend.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }
        
        public int? SenderId { get; set; } // Null if Admin sends to a non-registered user session (though prompt implies Registered User)
        public int? ReceiverId { get; set; } 
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        public bool IsAdminSender { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
    }
}
