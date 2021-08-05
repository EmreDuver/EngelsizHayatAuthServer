using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Dtos.LocationDtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using EngelsizHayatAuthServer.Data;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class LocationService : ILocationService
    {
        private readonly IGenericRepository<Location> _locationGenericRepository;
        private AppDbContext _context;
        private IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IGenericRepository<Location> locationGenericRepository, IUserService userService, AppDbContext context, IUnitOfWork unitOfWork)
        {
            _locationGenericRepository = locationGenericRepository;
            _userService = userService;
            _context = context;
            _unitOfWork = unitOfWork;
        }


        public async Task<Response<NoDataDto>> AddLocationAsync(Location location)
        {
            var result = GetByUserId(location.UserId);

            if (result.IsSuccessful) return Response<NoDataDto>.Fail(result.Error, 404, true);

            await _locationGenericRepository.AddAsync(location);

            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(201);
        }

        public Response<NoDataDto> DeleteLocationAsync(string userId)
        {
            var result = GetByUserId(userId);
            if (result == null) return Response<NoDataDto>.Fail(result.Error.Errors.ToString(), 404, true);

            _locationGenericRepository.Remove(result.Data);
            _unitOfWork.Commit();
            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> UpdateLocationAsync(UpdateLocationDto updateLocationDto)
        {
            var result = GetByUserId(updateLocationDto.UserId);

            if (!result.IsSuccessful) return Response<NoDataDto>.Fail(result.Error, 404, true);

            var locationNew = ObjectMapper.Mapper.Map<UpdateLocationDto, Location>(updateLocationDto, result.Data);

            _locationGenericRepository.Update(locationNew);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<Location>>> GetAllAsync()
        {
            var result = await _locationGenericRepository.GetAllAsync();

            return Response<IEnumerable<Location>>.Success(result, 200);
        }

        public Response<Location> GetByUserId(string userId)
        {
            var result = _locationGenericRepository.Where(x => x.UserId == userId).FirstOrDefault();
            if (result == null) return Response<Location>.Fail("Kullanıcıya kayıtlı bir lokasyon yok", 404, true);
            return Response<Location>.Success(result, 200);
        }

        public Response<Location> GetLocationById(int id)
        {
            var result = _locationGenericRepository.Where(x => x.Id == id).FirstOrDefault();
            if (result == null) return Response<Location>.Fail("Lokasyon bulunamadı", 404, true);
            return Response<Location>.Success(result, 200);

        }

        public Response<IEnumerable<UserAppDto>> GetNearbyLocation(string userId/*, int distance*/)
        {
            var result = _userService.GetUserById(userId);

            if (!result.IsSuccessful)
            {
                return Response<IEnumerable<UserAppDto>>.Fail(result.Error, 404);
            }

            var queryFirst = _locationGenericRepository.Where(x => x.UserId == userId).SingleOrDefault();

            decimal value = Convert.ToDecimal(0.12);

            var userLat = queryFirst.Latitude;
            var userLon = queryFirst.Longitude;

            var minusLat = userLat - value;
            var minusLon = userLon - value;

            var plusLat = userLat + value;
            var plusLon = userLon + value;

            var query = from u in _context.Users
                        join l in _context.Locations on u.Id equals l.UserId
                        where u.Id != userId
                        where (l.Latitude >= minusLat && l.Latitude <= plusLat) && (l.Longitude >= minusLon && l.Longitude <= plusLon)
                        select new UserAppDto 
                        {
                            Id = u.Id,
                            UserName = u.UserName,
                            FirstName = u.FirstName,
                            LastName = u.LastName,
                            PhotoPath = u.PhotoPath
                        };

            var locations = query.ToList();
            
            if (locations.Count == 0)
                return Response<IEnumerable<UserAppDto>>.Fail("Yakın çevrende biri yok", 404, true);

            return Response<IEnumerable<UserAppDto>>.Success(locations, 250);
        }




        //public double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        //{
        //    var R = 6371; //Haversine formülü
        //    var dLat = DegreeToRadian(lat2 - lat1);  
        //    var dLon = DegreeToRadian(lon2 - lon1);
        //    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(DegreeToRadian(lat1)) * Math.Cos(DegreeToRadian(lat2)) *Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        //    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        //    var d = R * c; 
        //    return d;
        //}

        //public double DegreeToRadian(double deg)
        //{
        //    return (deg * (Math.PI) / 180);
        //}
    }
}
