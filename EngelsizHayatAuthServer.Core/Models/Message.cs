using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Models
{
    public class Message
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Text { get; set; }
        public int Status  { get; set; }
        public long SendDateUnix { get; set; }
        public long ReadDateUnix { get; set; }
    }

    public enum MessageStatusType
    {
        Sent, Read
    }
}
