namespace EngelsizHayatAuthServer.Core.Dtos.CommentDtos
{
    public class DeleteCommentDto
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
    }
}