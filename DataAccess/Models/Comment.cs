using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.Now;


        [Required(ErrorMessage = "کاربر ضروری است")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "محصول ضروری است")]
        public int ProductId { get; set; }

        [Display(Name = "محصول")]
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
