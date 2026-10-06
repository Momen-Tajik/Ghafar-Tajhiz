using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Ghafar_Tajhiz.Models
{
    public class ResubmitReceiptDto
    {
        [Range(1, int.MaxValue)]
        public int BasketId { get; set; }

        [Required(ErrorMessage = "انتخاب رسید الزامی است.")]
        public IFormFile? Receipt { get; set; }
    }
}