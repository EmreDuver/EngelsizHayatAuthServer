using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Configurations;
using SharedLibrary.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly CustomTokenOption _TokenOption;

        public TokenService(UserManager<UserApp> userManager, IOptions<CustomTokenOption> options)
        {
            _userManager = userManager;
            _TokenOption = options.Value;
        }

        private string CreateRefreshToken()
        {
            // return Guid.NewGuid().ToString();

            var numberByte = new byte[32];
            using var rnd = RandomNumberGenerator.Create();

            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }

        private IEnumerable<Claim> GetClaims(UserApp userApp, List<String> audiences)
        {
            var addRole = "";
            var role =_userManager.GetRolesAsync(userApp).Result;
            addRole = role.Count==0 ? "User" : role[0];
            var userList = new List<Claim>
            {    //role kısmı burada yapılacak
                new Claim(ClaimTypes.NameIdentifier,userApp.Id),
                new Claim(ClaimTypes.Role,addRole),
                new Claim(JwtRegisteredClaimNames.Email,userApp.Email),
                new Claim(ClaimTypes.Name,userApp.UserName),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())//token id'si
            };
            userList.AddRange(audiences.Select(x => new Claim(JwtRegisteredClaimNames.Aud, x)));

            return userList;
        }

        public TokenDto CreateToken(UserApp userApp)
        {
            var accessTokenExpiration = DateTimeOffset.Now.AddMinutes(_TokenOption.AccessTokenExpiration).ToUnixTimeSeconds();
            var refreshTokenExpiration = DateTimeOffset.Now.AddMinutes(_TokenOption.RefreshTokenExpiration).ToUnixTimeSeconds();
            var securityKey = SignService.GetSymmetricSecurityKey(_TokenOption.SecurityKey);


            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _TokenOption.Issuer,
                expires: DateTime.Now.AddMinutes(_TokenOption.AccessTokenExpiration),
                notBefore: DateTime.Now,
                claims: GetClaims(userApp, _TokenOption.Audience),
                signingCredentials: signingCredentials);

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);
            var tokenDto = new TokenDto
            {
                AccessToken = token,
                RefreshToken = CreateRefreshToken(),
                AccessTokenExpiration = accessTokenExpiration,
                RefreshTokenExpiration = refreshTokenExpiration,
                UserId =  userApp.Id
            };
            return tokenDto;
        }

        public bool CheckIfTokenValid(long tokenExpirationTime)
        {
            if (DateTimeOffset.Now.ToUnixTimeSeconds() >= tokenExpirationTime)
                return false;

            return true;
        }
    }
}