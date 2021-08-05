namespace EngelsizHayatAuthServer.Core.Dtos.NestedCommentDtos
{
    public class DeleteNestedCommentDto
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string CommentId { get; set; }

        public string UserId { get; set; }
    }
}