using DataAccess.Enums;

namespace BusinessLogic.BasketServices.Models
{
    public class AdminOrderDetailDto
    {
        public int BasketId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public BasketStatus Status { get; set; }

        public DateTime Created { get; set; }

        public DateTime? PaidDate { get; set; }

        public DateTime? ReceiptUploadedAt { get; set; }

        public DateTime? PaymentVerifiedAt { get; set; }

        public string? ReceiptImage { get; set; }

        public string? PaymentRejectionReason { get; set; }

        public List<AdminOrderDetailItemDto> Items { get; set; } = new();
    }


    public class AdminOrderDetailItemDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        public int Qty { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice =>
            Qty * UnitPrice;
    }
}