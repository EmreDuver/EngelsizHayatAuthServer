using System;

namespace EngelsizHayatAuthServer.Core.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public long AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public long RefreshTokenExpiration { get; set; }
        public string UserId { get; set; }
    }
}