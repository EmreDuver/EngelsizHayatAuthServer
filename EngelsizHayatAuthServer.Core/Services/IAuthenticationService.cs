using EngelsizHayatAuthServer.Core.Dtos;
using SharedLibrary.Dtos;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface IAuthenticationService
    {
        Task<Response<TokenDto>> CreateTokenAsync(LoginDto loginDto);

        Task<Response<TokenDto>> CreateTokenByRefreshToken(string refreshToken);

        Task<Response<NoDataDto>> RevokeRefreshToken(string refreshToken);
    }
}