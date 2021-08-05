using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface ILikeService
    {
        Task<Response<NoDataDto>> Add(Like like);
        int GetAllLikeById(string id, int type);
        //bool AmILikeThisPost(string id,string userId, int type);
        List<string> UsersWhoLikePost(string id, int type);
        Task<Response<NoDataDto>> Delete(string postId, int type);
        Task<Response<UserListDto>> GetAllLikeByTypeAndId(LikeListDto likeListDto);
    }
}
