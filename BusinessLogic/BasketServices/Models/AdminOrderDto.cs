using DataAccess.Enums;

namespace BusinessLogic.BasketServices.Models
{
    public class AdminOrderDto
    {
        public int AdminOrderId { get; set; }

        public DateTime? PaidDate { get; set; }

        public int UserId { get; set; }

        public string Address { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public BasketStatus Status { get; set; }

        public string UserName { get; set; } = string.Empty;

        public List<string> Items { get; set; } = new();

        public string? ReceiptImage { get; set; }

        public DateTime? ReceiptUploadedAt { get; set; }

        public DateTime? PaymentVerifiedAt { get; set; }

        public string? PaymentRejectionReason { get; set; }
    }
}