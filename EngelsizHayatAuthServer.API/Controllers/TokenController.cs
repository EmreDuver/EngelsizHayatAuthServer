using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : CustomBaseController
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("checkiftokenvalid")]
        public IActionResult CheckIfTokenValid(TimeComparetionDto timeComperationDto)
        {
            var result = _tokenService.CheckIfTokenValid(timeComperationDto.TokenExpirationTime);

            if (!result)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
