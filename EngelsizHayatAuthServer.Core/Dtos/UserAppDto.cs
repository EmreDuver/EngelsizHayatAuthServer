using System;
using System.Collections.Generic;

namespace EngelsizHayatAuthServer.Core.Dtos
{
    public class UserAppDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDay { get; set; }
        public int GenderId { get; set; }
        public string PhotoPath { get; set; }
        public string Biography { get; set; }
    }
}