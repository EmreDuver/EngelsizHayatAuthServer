using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using SharedLibrary.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class LikeService : ILikeService
    {
        private readonly IGenericRepository<Like> _genericRepository;
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public LikeService(IGenericRepository<Like> genericRepository, IUserService userService, IUnitOfWork unitOfWork)
        {
            _genericRepository = genericRepository;
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<NoDataDto>> Add(Like like)
        {
            var user = _userService.GetUserById(like.UserId);

            if (!user.IsSuccessful)
                return Response<NoDataDto>.Fail(user.Error, 404, true);

            var isAlreadyLikes = _genericRepository.Where(x => x.Id == like.Id && x.Type == like.Type && x.UserId == like.UserId);
            if (isAlreadyLikes.Any())
            {
                return Response<NoDataDto>.Fail("Bu gönderi/yorum zaten beğenilmiştir.", 404, true);
            }

            await _genericRepository.AddAsync(like);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<NoDataDto>> Delete(string postId, int type)
        {
            var likes = _genericRepository.Where(x => x.Id == postId && x.Type == type).ToList();

            likes.ForEach(like =>
            {
                _genericRepository.Remove(like);
            });

            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }

        public int GetAllLikeById(string id, int type)
        {
            return _genericRepository.Where(x => x.Type == type && x.Id == id).Count();
        }

        //public bool AmILikeThisPost(string id, string userId, int type)
        //{
        //    return _genericRepository.Where(x => x.Type == type && x.Id == id && x.UserId == userId).Any();
        //}

        public List<string> UsersWhoLikePost(string id , int type)
        {
            return _genericRepository.Where(x => x.Type == type && x.Id == id).Select(users=>users.UserId).ToList();
        }

        public async Task<Response<UserListDto>> GetAllLikeByTypeAndId(LikeListDto likeListDto)
        {
            var userList = new UserListDto();

            userList = GetUserListDto(likeListDto.Id, likeListDto.Type);

            return Response<UserListDto>.Success(userList, 200);
        }

        private UserListDto GetUserListDto(string id, int type)
        {
            var listLike = _genericRepository.Where(x => x.Id == id && x.Type == type).ToList();

            var idList = new List<string>();
            foreach (var item in listLike)
            {
                idList.Add(item.UserId);
            }
            var userList = new UserListDto { Id = idList };

            return userList == null ? new UserListDto() : userList;
        }
    }
}