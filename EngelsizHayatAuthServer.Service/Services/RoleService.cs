using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Response<NoDataDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            var role = new AppRole()
            {
                Name = createRoleDto.Name
            };
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<NoDataDto>.Fail(new ErrorDto(errors, true), 400);
            }
            return Response<NoDataDto>.Success(204);
        }
    }
}
