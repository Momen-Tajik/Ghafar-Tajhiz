using BusinessLogic.ProductServices;
using DataAccess.Models;
using DataAccess.Repositories.CommentRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.CommentServices
{
    public class CommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<bool> CreateComment(string text, int productId, int userId , string userName)
        {
           
            var comment = new Comment()
            {
                Text=text,
                ProductId=productId,
                UserId=userId,
                Created = DateTime.Now,
                UserName=userName,
            };

            await _commentRepository.Add(comment);
            return true;
        }

        public async Task<bool> RemoveComment(int id)
        {
            //var comment = await _commentRepository.GetAll(a => a.CommentId == id).FirstOrDefaultAsync();
            await _commentRepository.Delete(id);
            return true;
        }
    }
}
