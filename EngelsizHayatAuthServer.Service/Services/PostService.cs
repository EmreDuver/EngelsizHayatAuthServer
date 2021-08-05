using EngelsizHayatAuthServer.Core.Dtos.PostDtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using EngelsizHayatAuthServer.Data;
using Microsoft.AspNetCore.Http;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class PostService : IPostService
    {
        private readonly IGenericRepository<Post> _genericRepository;
        private readonly IUserService _userService;
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILikeService _likeService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IUserService userService, AppDbContext context, IGenericRepository<Post> genericRepository, IUnitOfWork unitOfWork, ILikeService likeService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _context = context;
            _genericRepository = genericRepository;
            _unitOfWork = unitOfWork;
            _likeService = likeService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<NoDataDto>> AddPost(AddPostDto addPostDto)
        {
            var userCheck = CheckUserExistAsync(addPostDto.UserId);
            if (!userCheck.IsSuccessful)
            {
                return userCheck;
            }

            var addedPost = ObjectMapper.Mapper.Map<Post>(addPostDto);
            addedPost.Id = Guid.NewGuid().ToString();

            if (addPostDto.IsImageAttached)
            {
                byte[] bytes = Convert.FromBase64String(addPostDto.ImageBase64);

                var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "postImages", addedPost.Id + ".png");

                using (var imageFile = new FileStream(newPath, FileMode.Create))
                {
                    imageFile.Write(bytes, 0, bytes.Length);
                    imageFile.Flush();
                }

                addedPost.PhotoPath = "/postImages/"+addedPost.Id+".png";

            }
            else
            {
                if (String.IsNullOrEmpty(addPostDto.Text))
                {
                    return Response<NoDataDto>.Fail("Boş gönderi paylaşamazsınız!", 404, true);
                }
            }


            await _genericRepository.AddAsync(SetPostTime(addedPost));
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> ChangeActivityStatus(PostActivityDto postActivityDto)
        {
            var userCheck = CheckUserExistAsync(postActivityDto.UserId);
            if (!userCheck.IsSuccessful)
            {
                return userCheck;
            }

            var postExist = GetPostById(postActivityDto.Id).Result;

            if (!postExist.IsSuccessful)
                return Response<NoDataDto>.Fail(postExist.Error, 404, true);

            if (!postExist.Data.Active)
            {
                postExist.Data.Active = true;
            }
            else
            {
                postExist.Data.Active = false;
            }

            if (postActivityDto.UserId != postExist.Data.UserId)
                return Response<NoDataDto>.Fail("Başka bir kullanıcının gönderisinin aktlifliğini değiştiremezsin.", 400, true);

            _genericRepository.Update(ObjectMapper.Mapper.Map<PostActivityDto, Post>(postActivityDto, postExist.Data));
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<Post>> UpdateThePost(UpdatePostDto updatePostDto)
        {
            var postExist = GetPostById(updatePostDto.Id).Result;

            if (!postExist.IsSuccessful)
                return postExist;

            var postNew = ObjectMapper.Mapper.Map<UpdatePostDto, Post>(updatePostDto, postExist.Data);

            if (updatePostDto.UserId != postNew.UserId)
                return Response<Post>.Fail("Başka bir kullanıcının gönderisini güncelleyemezsin.", 400, true);

            _genericRepository.Update(SetPostTime(postNew));

            await _unitOfWork.CommitAsync();
            return Response<Post>.Success(200);
        }

        public Response<IEnumerable<PostDto>> GetAllPostByUserId(string userId)
        {
            var query = from u in _context.Users
                        join p in _context.Posts on u.Id equals p.UserId
                        where u.Id == userId
                        orderby p.CreateTime descending
                        select new PostDto()
                        {
                            UserName = u.UserName,
                            UserId = u.Id,
                            PhotoPath = u.PhotoPath,
                            CreateTime = (DateTimeOffset.Now.ToUnixTimeSeconds() - p.CreateTime).ToString(),
                            Id = p.Id,
                            Like = p.Like.ToString(),
                            Text = p.Text,
                            Active = p.Active,
                            IsImageAttached = p.IsImageAttached,
                            PostPhotoPath = p.PhotoPath
                        };

            if (!query.Any())
            {
                return Response<IEnumerable<PostDto>>.Fail("Kullanıcıya ait tek bir gönderi bile bulunamadı.", 404, true);
            }

            var newList = new List<PostDto>();

            foreach (var post in query)
            {
                post.CreateTime = getTimeInterval(post.CreateTime);
                newList.Add(post);
            }

            foreach (var item in newList)
            {
                item.UsersWhoLiked = _likeService.UsersWhoLikePost(item.Id, 0);
            }

            return Response<IEnumerable<PostDto>>.Success(newList.AsEnumerable(), 200);
        }

        public Response<IEnumerable<PostDto>> GetAllPostByUserIdForOtherUser(string userId)
        {
            var query = from u in _context.Users
                join p in _context.Posts on u.Id equals p.UserId
                where u.Id == userId 
                where p.Active
                orderby p.CreateTime descending
                select new PostDto()
                {
                    UserName = u.UserName,
                    UserId = u.Id,
                    PhotoPath = u.PhotoPath,
                    CreateTime = (DateTimeOffset.Now.ToUnixTimeSeconds() - p.CreateTime).ToString(),
                    Id = p.Id,
                    Like = p.Like.ToString(),
                    Text = p.Text,
                    Active = p.Active,
                    IsImageAttached = p.IsImageAttached,
                    PostPhotoPath = p.PhotoPath
                };

            if (!query.Any())
            {
                return Response<IEnumerable<PostDto>>.Fail("Kullanıcıya ait tek bir gönderi bile bulunamadı.", 404, true);
            }

            var newList = new List<PostDto>();

            foreach (var post in query)
            {
                post.CreateTime = getTimeInterval(post.CreateTime);
                newList.Add(post);
            }

            foreach (var item in newList)
            {
                item.UsersWhoLiked = _likeService.UsersWhoLikePost(item.Id, 0);
            }

            return Response<IEnumerable<PostDto>>.Success(newList.AsEnumerable(), 200);
        }

        public async Task<Response<Post>> GetPostById(string id)
        {
            var post = await _genericRepository.GetByIdAsync(id);
            if (post == null)
            {
                return Response<Post>.Fail("Gönderi bulunamadı", 404, true);
            }
            return Response<Post>.Success(post, 200);
        }

        public Response<IEnumerable<PostDto>> GetAllPostByCreateTime()
        {
            var currentUserId = _userService.GetUserById(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!currentUserId.IsSuccessful)
            {
                return Response<IEnumerable<PostDto>>.Fail("Kullanıcı hatası! Yeniden giriş yapın", 404, true);
            }

            var userName = currentUserId.Data.UserName;

            var query = from u in _context.Users
                        join p in _context.Posts on u.Id equals p.UserId
                        where u.UserName != userName
                        where p.Active
                        orderby p.CreateTime descending
                        select new PostDto()
                        {
                            UserName = u.UserName,
                            UserId = u.Id,
                            PhotoPath = u.PhotoPath,
                            CreateTime = (DateTimeOffset.Now.ToUnixTimeSeconds() - p.CreateTime).ToString(),
                            Id = p.Id,
                            Like = p.Like.ToString(),
                            Text = p.Text,
                            Active = p.Active,
                            IsImageAttached = p.IsImageAttached,
                            PostPhotoPath = p.PhotoPath
                        };

            if (!query.Any())
            {
                return Response<IEnumerable<PostDto>>.Fail("Sistemde tek bir gönderi bile bulunamadı.", 404, true);
            }

            var newList = new List<PostDto>();

            foreach (var post in query)
            {
                post.CreateTime = getTimeInterval(post.CreateTime);
                newList.Add(post);
            }

            foreach (var item in newList)
            {
                item.UsersWhoLiked = _likeService.UsersWhoLikePost(item.Id, 0);
            }

            return Response<IEnumerable<PostDto>>.Success(newList.AsEnumerable(), 200);
        }

        public Response<IEnumerable<PostDto>> GetAllPostByPopularity()
        {
            var currentUserId = _userService.GetUserById(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (!currentUserId.IsSuccessful)
            {
                return Response<IEnumerable<PostDto>>.Fail("Kullanıcı hatası! Yeniden giriş yapın", 404, true);
            }

            var userName = currentUserId.Data.UserName;

            var query = from u in _context.Users
                        join p in _context.Posts on u.Id equals p.UserId
                        where u.UserName != userName
                        where p.Active
                        orderby p.Like descending
                        select new PostDto()
                        {
                            UserName = u.UserName,
                            UserId = u.Id,
                            PhotoPath = u.PhotoPath,
                            CreateTime = (DateTimeOffset.Now.ToUnixTimeSeconds() - p.CreateTime).ToString(),
                            Id = p.Id,
                            Like = p.Like.ToString(),
                            Text = p.Text,
                            Active = p.Active,
                            IsImageAttached = p.IsImageAttached,
                            PostPhotoPath = p.PhotoPath
                        };

            if (!query.Any())
            {
                return Response<IEnumerable<PostDto>>.Fail("Sistemde tek bir gönderi bile bulunamadı.", 404, true);
            }

            var newList = new List<PostDto>();

            foreach (var post in query)
            {
                post.CreateTime = getTimeInterval(post.CreateTime);
                newList.Add(post);
            }

            foreach (var item in newList)
            {
                item.UsersWhoLiked = _likeService.UsersWhoLikePost(item.Id, 0);
            }

            return Response<IEnumerable<PostDto>>.Success(newList.AsEnumerable(), 200);
        }

        public Response<NoDataDto> Delete(DeletePostDto deletePostDto)
        {
            var userCheck = CheckUserExistAsync(deletePostDto.UserId);
            if (!userCheck.IsSuccessful)
            {
                return userCheck;
            }

            var postExist = GetPostById(deletePostDto.Id).Result;

            if (!postExist.IsSuccessful)
                return Response<NoDataDto>.Fail(postExist.Error, 404, true);

            if (deletePostDto.UserId != postExist.Data.UserId)
                return Response<NoDataDto>.Fail("Başka bir kullanıcının gönderisini silemezsin.", 400, true);

            if (postExist.Data.IsImageAttached)
            {
                var postPhotoPath1 = postExist.Data.PhotoPath.Replace("/", "").Replace("postImages", "");
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "postImages", postPhotoPath1);
                File.Delete(oldPath);
            }

            _likeService.Delete(postExist.Data.Id, 1);
            _genericRepository.Remove(postExist.Data);
            _unitOfWork.Commit();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> UpdateLike(string Id)
        {
            var post = await GetPostById(Id);

            if (!post.IsSuccessful)
                return Response<NoDataDto>.Fail(post.Error, 404, true);

            var likeCount = _likeService.GetAllLikeById(Id, 0);
            post.Data.Like = likeCount;
            _genericRepository.Update(post.Data);
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<IEnumerable<UserAppLikeDto>>> GetUserByWhoLikesPost(string Id)
        {
            var post = await GetPostById(Id);

            if (!post.IsSuccessful)
                return Response<IEnumerable<UserAppLikeDto>>.Fail(post.Error, 404, true);

            var result = await _likeService.GetAllLikeByTypeAndId(new LikeListDto { Id = post.Data.Id, Type = 0 });

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

        private Post SetPostTime(Post post)
        {
            post.CreateTime = DateTimeOffset.Now.ToUnixTimeSeconds();
            return post;
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
                timeInterval = $"{Math.Floor((double)createTimeToLong / 604800)} hafta önce";
            else if (createTimeToLong > 2592000 && createTimeToLong < 31536000)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 2592000)} ay önce";
            else if (createTimeToLong > 31536000)
                timeInterval = $"{Math.Floor((double)createTimeToLong / 31536000)} yıl önce";

            return timeInterval;
        }
    }
}