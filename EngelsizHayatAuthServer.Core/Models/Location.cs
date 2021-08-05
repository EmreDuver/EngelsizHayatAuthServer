using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Models
{
    public class Location 
    {
        public int Id { get; set; }
        public decimal Longitude  { get; set; }
        public decimal Latitude { get; set; }
        public string UserId { get; set; }
        public virtual UserApp UserApp { get; set; }
    }
}
