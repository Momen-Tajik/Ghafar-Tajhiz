using DataAccess.Data;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.CommentRepo
{
    public class CommentRepository : ICommentRepository
    {

        private readonly GhafarTajhizShopDbContext _context;

        public CommentRepository(GhafarTajhizShopDbContext context)
        {
            _context = context;
        }


        public async Task Add(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c=>c.CommentId==id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Comment> GetAll(Expression<Func<Comment, bool>> where = null)
        {
            var comments = _context.Comments.AsQueryable();
            if (where != null)
            {
                comments = comments.Where(where);
            }
            return comments;
        }

        public async Task<Comment> GetById(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == id);
        }
    }
}
