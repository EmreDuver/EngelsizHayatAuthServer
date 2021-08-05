using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface IRoleService
    {
        Task<Response<NoDataDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
    }
}
