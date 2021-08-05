using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NestedCommentController : CustomBaseController
    {
        private readonly INestedCommentService _nestedCommentService;

        public NestedCommentController(INestedCommentService nestedCommentService)
        {
            _nestedCommentService = nestedCommentService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(AddNestedCommentDto nestedCommentDto)
        {
            return ActionResultInstance(await _nestedCommentService.Add(nestedCommentDto));
        }

        [HttpPost("delete")]
        public IActionResult Delete(DeleteNestedCommentDto deleteNestedCommentDto)
        {
            return ActionResultInstance(_nestedCommentService.Delete(deleteNestedCommentDto));
        }

        [HttpPost("update")]
        public async Task<IActionResult> Update(UpdateNestedCommentDto updateNestedCommentDto)
        {
            return ActionResultInstance(await _nestedCommentService.Update(updateNestedCommentDto));
        }

        [HttpPost("updatelike")]
        public async Task<IActionResult> Update([FromBody] string nestedCommentId)
        {
            return ActionResultInstance(await _nestedCommentService.UpdateLike(nestedCommentId));
        }

        [HttpGet("getnestedcommentbyid")]
        public async Task<IActionResult> GetNestedCommentById([FromBody] string nestedCommentId)
        {
            return ActionResultInstance(await _nestedCommentService.GetNestedCommentById(nestedCommentId));
        }

        [HttpGet("getallnestedcommentsbycreatetime")]
        public async Task<IActionResult> GetAllNestedCommentsByCreateTime([FromBody] string nestedCommentId)
        {
            return ActionResultInstance(await _nestedCommentService.GetAllByCreateTime(nestedCommentId));
        }

        [HttpGet("getallnestedcommentsbypopularity")]
        public async Task<IActionResult> GetAllNestedCommentsByPopularity([FromBody] string nestedCommentId)
        {
            return ActionResultInstance(await _nestedCommentService.GetAllByPopularity(nestedCommentId));
        }

        [HttpGet("getuserbywholikesnestedcomment")]
        public async Task<IActionResult> GetUserByWhoLikesNestedComment([FromBody] string nestedCommentId)
        {
            return ActionResultInstance(await _nestedCommentService.GetUserByWhoLikesNestedComment(nestedCommentId));
        }
    }
}
