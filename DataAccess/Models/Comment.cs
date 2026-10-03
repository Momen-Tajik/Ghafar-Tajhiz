using DataAccess.Models;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Text { get; set; } = string.Empty;

        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;
    }
}