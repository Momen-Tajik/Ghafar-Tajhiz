using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.CommentRepo
{
    public interface ICommentRepository
    {
        IQueryable<Comment> GetAll(Expression<Func<Comment, bool>> where = null);
        Task<Comment> GetById(int id);
        Task Add(Comment comment);
        Task Delete(int id);
        Task Delete(Comment comment);
    }
}
