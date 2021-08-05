namespace EngelsizHayatAuthServer.Core.Models
{
    public class NestedComment
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public long CreateTime { get; set; }
        public string Text { get; set; }
        public long Like { get; set; }
        public virtual Post Post { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual UserApp UserApp { get; set; }
    }
}