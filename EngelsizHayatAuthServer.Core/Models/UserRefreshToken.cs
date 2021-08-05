using System;
using System.Collections.Generic;
using System.Text;

namespace EngelsizHayatAuthServer.Core.Models
{
    public class UserRefreshToken
    {
        public string UserId { get; set; }
        public string Code { get; set; }
        public long Expiration { get; set; }  
    }
}
