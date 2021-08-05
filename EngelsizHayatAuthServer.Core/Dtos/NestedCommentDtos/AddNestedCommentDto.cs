namespace EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos
{
    public class AddNestedCommentDto
    {
        public string PostId { get; set; }
        public string CommentId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}