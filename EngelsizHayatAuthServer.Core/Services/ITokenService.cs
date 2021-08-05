using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Models;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface ITokenService
    {
        TokenDto CreateToken(UserApp userApp);
        bool CheckIfTokenValid(long tokenExpirationTime);
    }
}