using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.CommentRepo
{
    public class CommentRepository : ICommentRepository
    {
        private readonly GhafarTajhizShopDbContext _context;

        public CommentRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Comment>> GetAllAsync()
        {
            return await _context.Comments
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .FirstOrDefaultAsync(c => c.CommentId == id);
        }

        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
        }

        public void Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
        }
    }
}