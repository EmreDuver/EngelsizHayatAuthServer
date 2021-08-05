using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.CommentDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface ICommentService
    {
        Task<Response<CommentDto>> Add(AddCommentDto addCommentDto);
        Response<NoDataDto> Delete(DeleteCommentDto deleteCommentDto);
        Response<NoDataDto> DeleteAllComments(string postId);
        Task<Response<Comment>> Update(UpdateCommentDto updateCommentDto);
        Task<Response<NoDataDto>> UpdateLike(string id);
        Task<Response<Comment>> GetCommentById(string commentId);
        Task<Response<IEnumerable<CommentDto>>> GetAllCommentByCreateTime(string postId);
        Task<Response<IEnumerable<Comment>>> GetAllCommentByPopularity(string postId);
        Task<Response<IEnumerable<UserAppLikeDto>>> GetUserByWhoLikesComment(string id);
    }
}
