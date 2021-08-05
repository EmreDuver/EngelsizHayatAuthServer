namespace EngelsizHayatAuthServer.Core.Dtos.PostDtos
{
    public class AddPostDto
    {
        public string UserId { get; set; }
        public string Text { get; set; }
        public bool IsImageAttached { get; set; }
        public string ImageBase64 { get; set; }
    }
}