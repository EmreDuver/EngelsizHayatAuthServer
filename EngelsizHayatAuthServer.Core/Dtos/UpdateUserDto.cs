using System;

namespace EngelsizHayatAuthServer.Core.Dtos
{
    public class UpdateUserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDay { get; set; }
        public int GenderId { get; set; }
    }
}