using System.ComponentModel.DataAnnotations;

namespace Ghafar_Tajhiz_Admin.Models
{
    public class StatusDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "شناسه سفارش نامعتبر است")]
        public int BasketId { get; set; }

        public bool Status { get; set; }
    }
}