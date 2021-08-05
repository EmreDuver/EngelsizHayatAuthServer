using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.CommentDtos
{
    public class AddCommentDto
    {
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}
