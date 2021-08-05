namespace EngelsizHayatAuthServer.Core.Dtos.CommentDtos
{
    public class UpdateCommentDto
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
    }
}