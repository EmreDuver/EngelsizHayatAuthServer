using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.LocationDtos
{
    public class UpdateLocationDto
    { 
        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }
        public string UserId { get; set; }
    }
}
