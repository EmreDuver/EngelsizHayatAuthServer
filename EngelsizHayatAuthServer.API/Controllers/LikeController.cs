using EngelsizHayatAuthServer.Core.Dtos.Util;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : CustomBaseController
    {
        private readonly ILikeService _likeService;

        public LikeController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        [HttpPost("add")] 
        public async Task<IActionResult> Add(Like like)
        {
            return ActionResultInstance(await _likeService.Add(like));
        }
    }
}