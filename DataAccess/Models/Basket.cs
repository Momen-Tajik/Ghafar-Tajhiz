using DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Basket
    {
        public int BasketId { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;

        public DateTime? PaidDate { get; set; }

        public int UserId { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [StringLength(20)]
        public string? MobileNumber { get; set; }

        public BasketStatus Status { get; set; }

        // ================================
        // Payment
        // ================================

        [StringLength(255)]
        public string? ReceiptImage { get; set; }

        public DateTime? ReceiptUploadedAt { get; set; }

        public DateTime? PaymentVerifiedAt { get; set; }

        [StringLength(500)]
        public string? PaymentRejectionReason { get; set; }

        // ================================

        public User? User { get; set; }

        public ICollection<BasketItem> BasketItems { get; set; }
            = new List<BasketItem>();
    }
}