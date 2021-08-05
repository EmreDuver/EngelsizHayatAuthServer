using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : CustomBaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost("createroleasync")]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            return ActionResultInstance(await _roleService.CreateRoleAsync(createRoleDto));
        }
    }
}
