using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class GenderService: IGenderService
    {
        private readonly IGenericRepository<Gender> _genderGenericRepository;
        public GenderService(IGenericRepository<Gender> genderGenericRepository)
        {
            _genderGenericRepository = genderGenericRepository;
        }

        public async Task<Response<IEnumerable<Gender>>> GetAllAsync()
        {
            var result = await _genderGenericRepository.GetAllAsync();

            return Response<IEnumerable<Gender>>.Success(result, 200);
        }

        public async Task<Response<Gender>> GetById(int genderId)
        {
            var result = await _genderGenericRepository.GetByIdAsync(genderId);

            if (result == null)
            {
                return Response<Gender>.Fail("Hata oluştu.", 404, true);

            }

            return Response<Gender>.Success(result, 200);
        }
    }
}
