using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.CommentDtos;
using EngelsizHayatAuthServer.Core.Dtos.PostDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using EngelsizHayatAuthServer.Data;
using SharedLibrary.Dtos;
using static System.String;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class CommentService : ICommentService
    {
        private readonly IGenericRepository<Comment> _genericRepository;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILikeService _likeService;
        private readonly IPostService _postService;

        public CommentService(IGenericRepository<Comment> genericRepository, IUserService userService, AppDbContext context, IUnitOfWork unitOfWork, ILikeService likeService, IPostService postService)
        {
            _genericRepository = genericRepository;
            _userService = userService;
            _context = context;
            _unitOfWork = unitOfWork;
            _likeService = likeService;
            _postService = postService;
        }

        public async Task<Response<CommentDto>> Add(AddCommentDto addCommentDto)
        {
            var userCheck = CheckUserExistAsync(addCommentDto.UserId);
            if (!userCheck.IsSuccessful)
                return Response<CommentDto>.Fail(userCheck.Error, 404, true);

            var postCheck = CheckPostExistAsync(addCommentDto.PostId);
            if (!postCheck.IsSuccessful)
                return Response<CommentDto>.Fail(postCheck.Error, 404, true);

            if (IsNullOrWhiteSpace(addCommentDto.Text) || IsNullOrEmpty(addCommentDto.Text))
                return Response<CommentDto>.Fail("Yorum boş olamaz.", 404, true);

            var addedComment = ObjectMapper.Mapper.Map<Comment>(addCommentDto);
            addedComment.Id = Guid.NewGuid().ToString();
            addedComment = SetCommentTime(addedComment);
            await _genericRepository.AddAsync(addedComment);
            await _unitOfWork.CommitAsync();

            var commentDto = ObjectMapper.Mapper.Map<CommentDto>(addedComment);
            var existUser = _userService.GetUserById(commentDto.UserId);
            commentDto.PhotoPath = existUser.Data.PhotoPath;
            commentDto.UserName = existUser.Data.UserName;
            commentDto.CreateTime = getTimeInterval("1");
            commentDto.UsersWhoLiked = new List<string>();
            commentDto.UsersWhoCanAllowDeleteMyComment = new List<string> { addCommentDto.UserId };

            return Response<CommentDto>.Success(commentDto, 200);
        }

        public Response<NoDataDto> Delete(DeleteCommentDto deleteCommentDto)
        {
            var userCheck = CheckUserExistAsync(deleteCommentDto.UserId);
            if (!userCheck.IsSuccessful)
                return userCheck;

            var postCheck = CheckPostExistAsync(deleteCommentDto.PostId);
            if (!postCheck.IsSuccessful)
                return postCheck;

            var commentExist = GetCommentById(deleteCommentDto.Id).Result;

            if (!commentExist.IsSuccessful)
                return Response<NoDataDto>.Fail(commentExist.Error, 404, true);

            if(!GetUsersWhoCanAllowToDeleteMyComment(deleteCommentDto.Id).Contains(deleteCommentDto.UserId))
                return Response<NoDataDto>.Fail("Başka bir kullanıcının yorumunu silemezsin.", 400, true);

            _genericRepository.Remove(commentExist.Data);
            _unitOfWork.Commit();
            _likeService.Delete(deleteCommentDto.Id, 1);
            return Response<NoDataDto>.Success(204);
        }

        public Response<NoDataDto> DeleteAllComments(string postId)
        {
            var postCheck = CheckPostExistAsync(postId);

            if (!postCheck.IsSuccessful)
                return postCheck;

            var allComments = GetAllCommentByPopularity(postId);

            if (allComments.Result.IsSuccessful)
            {
                if(!allComments.Result.Data.Any())
                    return Response<NoDataDto>.Success(204);
                foreach (var selectedComment in allComments.Result.Data)
                {
                    _likeService.Delete(selectedComment.Id, 1);
                    _genericRepository.Remove(selectedComment);
                    _unitOfWork.Commit();
                }
            }
            else
            {
                return Response<NoDataDto>.Fail("Result hatalı geldi",404,true);
            }


            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<Comment>> Update(UpdateCommentDto updateCommentDto)
        {
            var userCheck = CheckUserExistAsync(updateCommentDto.UserId);
            if (!userCheck.IsSuccessful)
                return Response<Comment>.Fail(userCheck.Error, 404, true);

            var postCheck = CheckPostExistAsync(updateCommentDto.PostId);
            if (!postCheck.IsSuccessful)
                return Response<Comment>.Fail(postCheck.Error, 404, true);

            var commentExist = GetCommentById(updateCommentDto.Id).Result;
            if (!commentExist.IsSuccessful)
                return Response<Comment>.Fail(commentExist.Error, 404, true);

            var commentNew = ObjectMapper.Mapper.Map<UpdateCommentDto, Comment>(updateCommentDto, commentExist.Data);

            if (updateCommentDto.UserId != commentNew.UserId)
                return Response<Comment>.Fail("Başka bir kullanıcının yorumunu düzenleyemezsin.", 403, true);

            _genericRepository.Update(SetCommentTime(commentNew));
            await _unitOfWork.CommitAsync();
            return Response<Comment>.Success(200);
        }

        public async Task<Response<NoDataDto>> UpdateLike(string id)
        {
            var comment = await GetCommentById(id);

            var postCheck = CheckPostExistAsync(comment.Data.PostId);
            if (!postCheck.IsSuccessful)
                return postCheck;

            if (!comment.IsSuccessful)
                return Response<NoDataDto>.Fail(comment.Error, 404, true);

            var likeCount = _likeService.GetAllLikeById(id,1);
            comment.Data.Like = likeCount;
            _genericRepository.Update(comment.Data);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<Comment>> GetCommentById(string commentId)
        {
            var comment = await _genericRepository.GetByIdAsync(commentId);
            if (comment == null)
                return Response<Comment>.Fail("Yorum bulunamadı", 404, true);

            return Response<Comment>.Success(comment, 200);
        }

        public async Task<Response<IEnumerable<CommentDto>>> GetAllCommentByCreateTime(string postId)
        {
            
            var postCheck = CheckPostExistAsync(postId);
            if (!postCheck.IsSuccessful)
                return Response<IEnumerable<CommentDto>>.Fail(postCheck.Error, 404, true);

            var query = from c in _context.Comments
                join u in _context.Users on c.UserId equals u.Id
                where c.PostId == postId
                orderby c.CreateTime descending
                select new CommentDto()
                {
                    Id = c.Id,
                    PostId = postId,
                    UserId = u.Id,
                    UserName=u.UserName,
                    PhotoPath = u.PhotoPath,
                    CreateTime = (DateTimeOffset.Now.ToUnixTimeSeconds() - c.CreateTime).ToString(),
                    Like = c.Like,
                    Text = c.Text
                };

            if (!query.Any())
            {
                return Response<IEnumerable<CommentDto>>.Fail("Yorum bulunumadı.", 404, true);
            }

            var newList = new List<CommentDto>();

            foreach (var comment in query)
            {
                comment.CreateTime = getTimeInterval(comment.CreateTime);
                newList.Add(comment);
            }

            foreach (var item in newList)
            {
                item.UsersWhoLiked = _likeService.UsersWhoLikePost(item.Id, 1);
                item.UsersWhoCanAllowDeleteMyComment = GetUsersWhoCanAllowToDeleteMyComment(item.Id);
            }

            return Response<IEnumerable<CommentDto>>.Success(newList, 200);
        }

        public async Task<Response<IEnumerable<Comment>>> GetAllCommentByPopularity(string postId)
        {
            var postCheck = CheckPostExistAsync(postId);
            if (!postCheck.IsSuccessful)
                return Response<IEnumerable<Comment>>.Fail(postCheck.Error, 404, true);

            var getComments = _genericRepository.Where(x => x.PostId == postId).OrderByDescending(x => x.Like).ToList();

            return Response<IEnumerable<Comment>>.Success(getComments, 200);
        }
        

        public async Task<Response<IEnumerable<UserAppLikeDto>>> GetUserByWhoLikesComment(string id)
        {
            var comment = GetCommentById(id).Result;

            if (!comment.IsSuccessful)
                return Response<IEnumerable<UserAppLikeDto>>.Fail(comment.Error, 404, true);

            var postCheck = CheckPostExistAsync(comment.Data.PostId);
            if (!postCheck.IsSuccessful)
                return Response<IEnumerable<UserAppLikeDto>>.Fail(postCheck.Error, 404, true);

            var result = await _likeService.GetAllLikeByTypeAndId(new LikeListDto { Id = comment.Data.Id, Type = 1 });

            var userAppLikeDtoList = new List<UserAppLikeDto>();
            foreach (var item in result.Data.Id)
            {
                userAppLikeDtoList.Add(_userService.GetUserByWhoLikesCommentAndPost(item).Result.Data);
            }

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

        private Comment SetCommentTime(Comment comment)
        {
            comment.CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            return comment;
        }

        private string getTimeInterval(string time)
        {
            var createTimeToLong = Convert.ToInt64(time);
            var timeInterval = "";

            if (createTimeToLong < 60)
                timeInterval = $"{createTimeToLong} saniye önce";
            else if (createTimeToLong > 60 && createTimeToLong < 3600)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 60)} dakika önce";
            else if (createTimeToLong > 3600 && createTimeToLong < 86400)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 3600)} saat önce";
            else if (createTimeToLong > 86400 && createTimeToLong < 604800)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 86400)} gün önce";
            else if (createTimeToLong > 604800 && createTimeToLong < 2592000)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 86400)} hafta önce";
            else if (createTimeToLong > 2592000 && createTimeToLong < 31536000)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 2592000)} ay önce";
            else if (createTimeToLong > 31536000)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 31536000)} yıl önce";

            return timeInterval;
        }

        private List<string> GetUsersWhoCanAllowToDeleteMyComment(string commentId)
        {
            var query = from c in _context.Comments
                join p in _context.Posts on c.PostId equals p.Id
                where c.Id == commentId
                select new List<String>
                {
                    p.UserId, c.UserId
                };

            return query.FirstOrDefault().ToList();
        }
    }
}
