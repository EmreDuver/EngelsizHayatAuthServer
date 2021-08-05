using EngelsizHayatAuthServer.Core.Dtos.PostDtos;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : CustomBaseController
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [Authorize]
        [HttpGet("getallpostbycreatetime")]
        public IActionResult GetAllPostByCreateTime()
        {
            return ActionResultInstance(_postService.GetAllPostByCreateTime());
        }

        [Authorize]
        [HttpGet("getallpostbypopularity")]
        public IActionResult GetAllPostByPopularity()
        {
            return ActionResultInstance(_postService.GetAllPostByPopularity());
        }

        [Authorize]
        [HttpPost("getallpostbyuserid")]
        public IActionResult GetAllPostByUserId([FromBody] string userId)
        {
            return ActionResultInstance(_postService.GetAllPostByUserId(userId));
        }

        [Authorize]
        [HttpPost("getallpostbyuseridforotheruser")]
        public IActionResult GetAllPostByUserIdForOtherUser([FromBody] string userId)
        {
            return ActionResultInstance(_postService.GetAllPostByUserIdForOtherUser(userId));
        }

        [Authorize]
        [HttpGet("getpostbyid")]
        public async Task<IActionResult> GetPostById([FromBody] string postId)
        {
            return ActionResultInstance(await _postService.GetPostById(postId));
        }

        [Authorize]
        [HttpPost("getuserbywholikespost")]
        public async Task<IActionResult> GetUserByWhoLikesPost([FromBody] string postId)
        {
            return ActionResultInstance(await _postService.GetUserByWhoLikesPost(postId));
        }

        [Authorize(Roles = "Doctor,Admin")]
        [HttpPost("add")]
        public async Task<IActionResult> Add(AddPostDto postDto)
        {
            return ActionResultInstance(await _postService.AddPost(postDto));
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> Update(UpdatePostDto postDto)
        {
            return ActionResultInstance(await _postService.UpdateThePost(postDto));
        }

        [Authorize]
        [HttpPost("updatelike")]
        public async Task<IActionResult> Update([FromBody] string postId)
        {
            return ActionResultInstance(await _postService.UpdateLike(postId));
        }

        [Authorize]
        [HttpPost("changeactivitypost")]
        public async Task<IActionResult> Deactive(PostActivityDto postActivityDto)
        {
            return ActionResultInstance(await _postService.ChangeActivityStatus(postActivityDto));
        }

        [Authorize]
        [HttpPost("delete")]
        public IActionResult Delete(DeletePostDto deletePostDto)
        {
            return ActionResultInstance(_postService.Delete(deletePostDto));
        }

        //[HttpPost("amilikethispost")]
        //public async Task<IActionResult> AmILikeThisPost(AmILikeThisPostDtoSend amILikeThisPostDtoSend)
        //{
        //    return ActionResultInstance(await _postService.AmILikeThisPost(amILikeThisPostDtoSend));
        //}
    }
}