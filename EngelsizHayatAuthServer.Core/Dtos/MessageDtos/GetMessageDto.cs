using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.MessageDtos
{
    public class GetMessageDto
    {
        public string ReceiverId { get; set; }
        public string SenderId { get; set; }
    }
}
