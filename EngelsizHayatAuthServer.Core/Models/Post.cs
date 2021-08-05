using System.Collections.Generic;

namespace EngelsizHayatAuthServer.Core.Models
{
    public class Post
    {
        public string Id { get; set; }
        public long CreateTime { get; set; }
        public string Text { get; set; }
        public string UserId { get; set; }
        public long Like { get; set; }
        public bool Active { get; set; }
        public bool IsImageAttached { get; set; }
        public string PhotoPath { get; set; }
        public virtual UserApp UserApp { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<NestedComment> NestedComments { get; set; }
    }
}