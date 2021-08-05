using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Dtos.LocationDtos;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface ILocationService
    {
        Task<Response<NoDataDto>> AddLocationAsync(Location location);
        Response<NoDataDto> DeleteLocationAsync(string userId);
        Task<Response<NoDataDto>> UpdateLocationAsync(UpdateLocationDto updateLocationDto);
        Task<Response<IEnumerable<Location>>> GetAllAsync();
        Response<Location> GetByUserId(string userId);
        Response<Location> GetLocationById(int userId);
        Response<IEnumerable<UserAppDto>> GetNearbyLocation(string userId);
    }
}
