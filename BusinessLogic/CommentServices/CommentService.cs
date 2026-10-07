using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.CommentServices
{
    public class CommentService
    {
        private readonly GhafarTajhizShopDbContext _context;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            GhafarTajhizShopDbContext context,
            ILogger<CommentService> logger)
        {
            _context = context;
            _logger = logger;
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create comment for product {ProductId} by user {UserId}",
                    productId,
                    userId);

                return false;
            }

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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to remove comment {CommentId} by user {UserId}",
                    commentId,
                    userId);

                return false;
            }

            return true;
        }
    }
}