using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Enums;

namespace DataAccess.Models
{
    public class Basket
    {
        [Key]
        public int BasketId { get; set; }


        public DateTime Created { get; set; } = DateTime.Now;


        public DateTime? PaidDate { get; set; }


        public int UserId { get; set; }


        [MaxLength(500)]
        public string? Address { get; set; }


        [MaxLength(20)]
        public string? MobileNumber { get; set; }


        [Display(Name = "وضعیت سبد")]
        [Required]
        public BasketStatus Status { get; set; }


        // =========================================
        // Payment Receipt
        // =========================================

        [MaxLength(500)]
        public string? ReceiptImage { get; set; }

        public DateTime? ReceiptUploadedAt { get; set; }

        public DateTime? PaymentVerifiedAt { get; set; }

        [MaxLength(1000)]
        public string? PaymentRejectionReason { get; set; }


        // =========================================
        // Relations
        // =========================================

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }


        public ICollection<BasketItem> BasketItems { get; set; }
            = new List<BasketItem>();
    }
}