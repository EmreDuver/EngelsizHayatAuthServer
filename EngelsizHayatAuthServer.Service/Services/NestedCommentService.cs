using EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using EngelsizHayatAuthServer.Data;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class NestedCommentService : INestedCommentService
    {
        private readonly IGenericRepository<NestedComment> _genericRepository;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILikeService _likeService;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public NestedCommentService(ICommentService commentService, IPostService postService, ILikeService likeService, IUnitOfWork unitOfWork, AppDbContext context, IUserService userService, IGenericRepository<NestedComment> genericRepository)
        {
            _commentService = commentService;
            _postService = postService;
            _likeService = likeService;
            _unitOfWork = unitOfWork;
            _context = context;
            _userService = userService;
            _genericRepository = genericRepository;
        }

        public async Task<Response<NoDataDto>> Add(AddNestedCommentDto addNestedCommentDto)
        {
            var userCheck = CheckUserExistAsync(addNestedCommentDto.UserId);
            if (!userCheck.IsSuccessful)
                return userCheck;

            var postCheck = CheckPostExistAsync(addNestedCommentDto.PostId);
            if (!postCheck.IsSuccessful)
                return postCheck;

            var commentCheck = CheckCommentExistAsync(addNestedCommentDto.CommentId);
            if (!commentCheck.IsSuccessful)
                return commentCheck;

            var addedNestedComment = ObjectMapper.Mapper.Map<NestedComment>(addNestedCommentDto);
            addedNestedComment.Id = Guid.NewGuid().ToString();

            await _genericRepository.AddAsync(SetNestedCommentTime(addedNestedComment));
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public Response<NoDataDto> Delete(DeleteNestedCommentDto deleteCommentDto)
        {
            var userCheck = CheckUserExistAsync(deleteCommentDto.UserId);
            if (!userCheck.IsSuccessful)
                return userCheck;

            var postCheck = CheckPostExistAsync(deleteCommentDto.PostId);
            if (!postCheck.IsSuccessful)
                return postCheck;

            var commentCheck = CheckCommentExistAsync(deleteCommentDto.CommentId);
            if (!commentCheck.IsSuccessful)
                return commentCheck;

            var nestedCommentExist = GetNestedCommentById(deleteCommentDto.Id).Result;

            if (!nestedCommentExist.IsSuccessful)
                return Response<NoDataDto>.Fail(nestedCommentExist.Error, 404, true);

            if (deleteCommentDto.UserId != nestedCommentExist.Data.UserId)
                return Response<NoDataDto>.Fail("Başka bir kullanıcının yorumunu silemezsin.", 400, true);

            _genericRepository.Remove(nestedCommentExist.Data);
            _unitOfWork.Commit();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NestedComment>> Update(UpdateNestedCommentDto updateNestedCommentDto)
        {
            var userCheck = CheckUserExistAsync(updateNestedCommentDto.UserId);
            if (!userCheck.IsSuccessful)
                return Response<NestedComment>.Fail(userCheck.Error, 403, true);

            var postCheck = CheckPostExistAsync(updateNestedCommentDto.PostId);
            if (!postCheck.IsSuccessful)
                return Response<NestedComment>.Fail(postCheck.Error, 403, true);

            var commentCheck = CheckCommentExistAsync(updateNestedCommentDto.CommentId);
            if (!commentCheck.IsSuccessful)
                return Response<NestedComment>.Fail(commentCheck.Error, 403, true);

            var nestedCommentExist = GetNestedCommentById(updateNestedCommentDto.Id).Result;

            if (!nestedCommentExist.IsSuccessful)
                return Response<NestedComment>.Fail(nestedCommentExist.Error, 403, true);

            var nestedCommentNew = ObjectMapper.Mapper.Map<UpdateNestedCommentDto, NestedComment>(updateNestedCommentDto, nestedCommentExist.Data);

            if (updateNestedCommentDto.UserId != nestedCommentNew.UserId)
                return Response<NestedComment>.Fail("Başka bir kullanıcının yorumunu düzenleyemezsin.", 403, true);

            _genericRepository.Update(SetNestedCommentTime(nestedCommentNew));
            await _unitOfWork.CommitAsync();
            return Response<NestedComment>.Success(200);
        }

        public async Task<Response<NoDataDto>> UpdateLike(string id)
        {
            var nestedComment = await GetNestedCommentById(id);

            var postCheck = CheckPostExistAsync(nestedComment.Data.PostId);
            if (!postCheck.IsSuccessful)
                return postCheck;

            var commentCheck = CheckCommentExistAsync(nestedComment.Data.CommentId);
            if (!commentCheck.IsSuccessful)
                return commentCheck;

            if (!nestedComment.IsSuccessful)
                return Response<NoDataDto>.Fail(nestedComment.Error, 403, true);

            var likeCount = _likeService.GetAllLikeById(id, 2);
            nestedComment.Data.Like = likeCount;
            _genericRepository.Update(nestedComment.Data);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<NestedComment>> GetNestedCommentById(string id)
        {
            var nestedComment = await _genericRepository.GetByIdAsync(id);
            if (nestedComment == null)
                return Response<NestedComment>.Fail("Yorum bulunamadı", 404, true);

            return Response<NestedComment>.Success(nestedComment, 200);
        }

        public async Task<Response<IEnumerable<NestedComment>>> GetAllByCreateTime(string commentId)
        {
            var commentCheck = CheckCommentExistAsync(commentId);
            if (!commentCheck.IsSuccessful)
                return Response<IEnumerable<NestedComment>>.Fail(commentCheck.Error, 404, true);

            var getComments = _genericRepository.Where(x => x.CommentId == commentId).OrderByDescending(x => x.CreateTime).AsEnumerable();

            return Response<IEnumerable<NestedComment>>.Success(getComments, 200);
        }

        public async Task<Response<IEnumerable<NestedComment>>> GetAllByPopularity(string commentId)
        {
            var commentCheck = CheckCommentExistAsync(commentId);
            if (!commentCheck.IsSuccessful)
                return Response<IEnumerable<NestedComment>>.Fail(commentCheck.Error, 404, true);

            var getComments = _genericRepository.Where(x => x.CommentId == commentId).OrderByDescending(x => x.Like).AsEnumerable();

            return Response<IEnumerable<NestedComment>>.Success(getComments, 200);
        }

        public async Task<Response<IEnumerable<UserAppLikeDto>>> GetUserByWhoLikesNestedComment(string id)
        {
            var nestedComment = GetNestedCommentById(id).Result;

            if (!nestedComment.IsSuccessful)
                return Response<IEnumerable<UserAppLikeDto>>.Fail(nestedComment.Error, 404, true);

            var commentCheck = CheckCommentExistAsync(nestedComment.Data.CommentId);
            if (!commentCheck.IsSuccessful)
                return Response<IEnumerable<UserAppLikeDto>>.Fail(commentCheck.Error, 404, true);

            var postCheck = CheckPostExistAsync(nestedComment.Data.PostId);
            if (!postCheck.IsSuccessful)
                return Response<IEnumerable<UserAppLikeDto>>.Fail(postCheck.Error, 404, true);

            var result = await _likeService.GetAllLikeByTypeAndId(new LikeListDto { Id = nestedComment.Data.Id, Type = 2 });

            var userAppLikeDtoList = new List<UserAppLikeDto>();
            foreach (var item in result.Data.Id)
                userAppLikeDtoList.Add(_userService.GetUserByWhoLikesCommentAndPost(item).Result.Data);

            return Response<IEnumerable<UserAppLikeDto>>.Success(userAppLikeDtoList, 200);
        }

        private Response<NoDataDto> CheckUserExistAsync(string userId)
        {
            var user = _userService.GetUserById(userId);
            if (!user.IsSuccessful)
            {
                return Response<NoDataDto>.Fail(user.Error, user.StatusCode, true);
            }
            return Response<NoDataDto>.Success(user.StatusCode);
        }

        private Response<NoDataDto> CheckPostExistAsync(string postId)
        {
            var post = _postService.GetPostById(postId).Result;
            if (!post.IsSuccessful)
            {
                return Response<NoDataDto>.Fail(post.Error, post.StatusCode, true);
            }
            return Response<NoDataDto>.Success(post.StatusCode);
        }

        private Response<NoDataDto> CheckCommentExistAsync(string commentId)
        {
            var comment = _commentService.GetCommentById(commentId).Result;
            if (!comment.IsSuccessful)
            {
                return Response<NoDataDto>.Fail(comment.Error, comment.StatusCode, true);
            }
            return Response<NoDataDto>.Success(comment.StatusCode);
        }

        private NestedComment SetNestedCommentTime(NestedComment nestedComment)
        {
            nestedComment.CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            return nestedComment;
        }
    }
}