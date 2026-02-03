using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    }
}
