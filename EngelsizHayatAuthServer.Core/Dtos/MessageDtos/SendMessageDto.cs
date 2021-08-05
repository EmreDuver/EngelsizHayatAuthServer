using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.MessageDtos
{
    public class SendMessageDto
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string Message { get; set; }
    }
}
