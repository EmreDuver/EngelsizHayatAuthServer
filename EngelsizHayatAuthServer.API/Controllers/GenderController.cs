using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : CustomBaseController
    {
        private readonly IServiceGeneric<Gender, Gender> _genderService;

        public GenderController(IServiceGeneric<Gender, Gender> genderService)
        {
            _genderService = genderService;
        }

        [HttpGet("GetAllGenders")]
        public async Task<IActionResult> GetAllGenders()
        {
            return ActionResultInstance(await _genderService.GetAllAsync());
        }

        [HttpGet("GetGenderById")]
        public async Task<IActionResult> GetGenderById(int id)
        {
            return ActionResultInstance(await _genderService.GetByIdAsync(id));
        }
       
    }
}
