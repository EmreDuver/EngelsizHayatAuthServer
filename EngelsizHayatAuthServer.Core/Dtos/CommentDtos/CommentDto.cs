using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Dtos.CommentDtos
{
    public class CommentDto
    { 
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string CreateTime { get; set; }
        public string Text { get; set; }
        public long Like { get; set; }
        public string UserName { get; set; }
        public string PhotoPath { get; set; }
        public List<string> UsersWhoLiked { get; set; }
        public List<string> UsersWhoCanAllowDeleteMyComment { get; set; }

    }
}
