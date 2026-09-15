using DataAccess.Models;

namespace DataAccess.Repositories.CommentRepo
{
    public interface ICommentRepository
    {
        Task<IReadOnlyList<Comment>> GetAllAsync();
        Task<Comment?> GetByIdAsync(int id);
        Task AddAsync(Comment comment);
        void Delete(Comment comment);
    }
}