using EngelsizHayatAuthServer.Core.Dtos.CommentDtos;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : CustomBaseController
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddCommentDto commentDto)
        {
            return ActionResultInstance(await _commentService.Add(commentDto));
        }

        [HttpPost("delete")]
        public IActionResult Delete(DeleteCommentDto deleteCommentDto)
        {
            return ActionResultInstance(_commentService.Delete(deleteCommentDto));
        }
        [HttpPost("deleteallcomments")] 
        public IActionResult DeleteAllComments([FromBody] string postId)
        {
            return ActionResultInstance(_commentService.DeleteAllComments(postId));
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(UpdateCommentDto updateCommentDto)
        {
            return ActionResultInstance(await _commentService.Update(updateCommentDto));
        }

        [HttpPost("updatelike")]
        public async Task<IActionResult> Update([FromBody] string commentId)
        {
            return ActionResultInstance(await _commentService.UpdateLike(commentId));
        }

        [HttpGet("getcommentbyid")]
        public async Task<IActionResult> GetCommentById([FromBody] string commentId)
        {
            return ActionResultInstance(await _commentService.GetCommentById(commentId));
        }

        [HttpGet("getallcommentsbycreatetime/{postId}")]
        public async Task<IActionResult> GetAllCommentsByCreateTime(string postId)
        {
            return ActionResultInstance(await _commentService.GetAllCommentByCreateTime(postId));
        }

        [HttpGet("getallcommentsbypopularity")]
        public async Task<IActionResult> GetAllCommentsByPopularity([FromBody] string commentId)
        {
            return ActionResultInstance(await _commentService.GetAllCommentByPopularity(commentId));
        }

        [HttpGet("getuserbywholikescomment")]
        public async Task<IActionResult> GetUserByWhoLikesComment([FromBody] string commentId)
        {
            return ActionResultInstance(await _commentService.GetUserByWhoLikesComment(commentId));
        }
    }
}