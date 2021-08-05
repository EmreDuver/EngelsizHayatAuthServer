using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.MessageDtos
{
    public class MessageDto
    {
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public string SendDateUnix { get; set; }
    }
}
