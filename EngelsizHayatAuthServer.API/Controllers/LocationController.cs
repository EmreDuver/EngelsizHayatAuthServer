using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Dtos.LocationDtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : CustomBaseController
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("getalllocations")]
        public async Task<IActionResult> GetAllLocations()
        {
            return ActionResultInstance(await _locationService.GetAllAsync());
        }
        [HttpGet("getlocationbyid")]
        public IActionResult GetLocationById([FromBody]int id)
        {
            return ActionResultInstance(_locationService.GetLocationById(id));
        }
        [HttpGet("getbyuserid")]
        public IActionResult GetByUserId([FromBody]string userId)
        {
            return ActionResultInstance(_locationService.GetByUserId(userId));
        }

        [HttpPost("addlocation")]
        public async Task<IActionResult> AddLocation(Location location)
        {
            return ActionResultInstance(await _locationService.AddLocationAsync(location));
        }

        [HttpPost("updatelocation")]
        public async Task<IActionResult> UpdateLocation(UpdateLocationDto updateLocationDto)
        {
            return ActionResultInstance( await _locationService.UpdateLocationAsync(updateLocationDto));
        }

        [HttpPost("deletelocation")]
        public  IActionResult DeleteLocation([FromBody]string userId)
        {
            return ActionResultInstance(_locationService.DeleteLocationAsync(userId));
        }

        [HttpPost("getnearbylocations")]
        public IActionResult GetNearbyLocations([FromBody]string userId)
        {
            return ActionResultInstance(_locationService.GetNearbyLocation(userId));
        }
    }
}
