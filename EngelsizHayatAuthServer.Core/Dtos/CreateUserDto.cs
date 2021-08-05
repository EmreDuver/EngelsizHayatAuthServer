using System;
using System.Collections.Generic;
using System.Text;

namespace EngelsizHayatAuthServer.Core.Dtos
{
    public class CreateUserDto
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
