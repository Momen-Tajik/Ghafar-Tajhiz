using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.CommentServices
{
    public class CommentService
    {
        private readonly GhafarTajhizShopDbContext _context;

        public CommentService(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateComment(
            string text,
            int productId,
            int userId)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            var productExists = await _context.Products
                .AnyAsync(p => p.ProductId == productId);

            if (!productExists)
                return false;

            var comment = new Comment
            {
                Text = text.Trim(),
                ProductId = productId,
                UserId = userId,
                Created = DateTime.Now
            };

            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveComment(
            int commentId,
            int userId)
        {
            var comment = await _context.Comments
                .FirstOrDefaultAsync(c =>
                    c.CommentId == commentId &&
                    c.UserId == userId);

            if (comment == null)
                return false;

            _context.Comments.Remove(comment);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}