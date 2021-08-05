using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.MessageDtos
{
    public class MessageClientDto
    {
        public string MessageId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
