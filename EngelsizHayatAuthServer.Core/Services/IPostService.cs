using EngelsizHayatAuthServer.Core.Dtos.PostDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface IPostService
    {
        Task<Response<NoDataDto>> AddPost(AddPostDto addPostDto);
        Task<Response<NoDataDto>> ChangeActivityStatus(PostActivityDto postActivityDto);
        Task<Response<Post>> UpdateThePost(UpdatePostDto updatePostDto);
        Response<IEnumerable<PostDto>> GetAllPostByUserId(string userId);
        Response<IEnumerable<PostDto>> GetAllPostByUserIdForOtherUser(string userId);
        Task<Response<Post>> GetPostById(string id);
        Response<IEnumerable<PostDto>> GetAllPostByCreateTime();
        Response<IEnumerable<PostDto>> GetAllPostByPopularity();
        Response<NoDataDto> Delete(DeletePostDto deletePostDto);
        Task<Response<NoDataDto>> UpdateLike(string id);
        Task<Response<IEnumerable<UserAppLikeDto>>> GetUserByWhoLikesPost(string id);
    }
}