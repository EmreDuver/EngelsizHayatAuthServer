using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.MessageDtos
{
    public class MessageHistoryDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhotoPath { get; set; }
        public string MessageText { get; set; }
        public string MessageDate { get; set; }
    }
}
