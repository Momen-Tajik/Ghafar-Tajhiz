using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz_Admin.Models
{
    public class RejectPaymentDto
    {
        [Range(1, int.MaxValue)]
        public int BasketId { get; set; }

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;
    }
}