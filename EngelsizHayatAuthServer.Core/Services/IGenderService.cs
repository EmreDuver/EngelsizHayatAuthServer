using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface IGenderService
    {
        Task<Response<IEnumerable<Gender>>> GetAllAsync();
        Task<Response<Gender>> GetById(int genderId);
    }
}
