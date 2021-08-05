using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos
{
    public class UpdateNestedCommentDto
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}
