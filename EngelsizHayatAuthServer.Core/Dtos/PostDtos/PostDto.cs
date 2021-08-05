using System.Collections.Generic;

namespace EngelsizHayatAuthServer.Core.Dtos.PostDtos
{
    public class PostDto
    {
        public string Id { get; set; }
        public string CreateTime { get; set; }
        public string Text { get; set; }
        public string Like { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string PhotoPath { get; set; }
        public bool Active { get; set; }
        public List<string> UsersWhoLiked { get; set; }
        public bool IsImageAttached { get; set; }
        public string PostPhotoPath { get; set; }
    }
}