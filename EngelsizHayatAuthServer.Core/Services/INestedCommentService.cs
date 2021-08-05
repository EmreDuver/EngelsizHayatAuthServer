using System.Collections.Generic;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface INestedCommentService
    {
        Task<Response<NoDataDto>> Add(AddNestedCommentDto addCommentDto);
        Response<NoDataDto> Delete(DeleteNestedCommentDto deleteCommentDto);
        Task<Response<NestedComment>> Update(UpdateNestedCommentDto updateCommentDto);
        Task<Response<NoDataDto>> UpdateLike(string id);
        Task<Response<NestedComment>> GetNestedCommentById(string id);
        Task<Response<IEnumerable<NestedComment>>> GetAllByCreateTime(string commentId);
        Task<Response<IEnumerable<NestedComment>>> GetAllByPopularity(string commentId);
        Task<Response<IEnumerable<UserAppLikeDto>>> GetUserByWhoLikesNestedComment(string id);
    }
}