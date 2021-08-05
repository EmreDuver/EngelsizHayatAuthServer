using System;
using System.Collections.Generic;
using EngelsizHayatAuthServer.Core.Dtos;
using SharedLibrary.Dtos;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.BigData;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Models;
using Microsoft.AspNetCore.Http;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
        Task<Response<UserAppDto>> GetUserByIdForOtherUserProfileAsync(string userId);
        Task<Response<NoDataDto>> VerifyEmailAsync(string userId, string token);
        Task<Response<NoDataDto>> ResetPasswordRequestAsync(string email);
        Task<Response<NoDataDto>> ResetPasswordAsync(string userId, string token);
        Task<Response<NoDataDto>> UpdateProfileAsync(UpdateUserDto updateUserDto);
        Task<Response<NoDataDto>> EmailChangeRequestAsync(EmailChangeDto emailChangeDto);
        Task<Response<NoDataDto>> EmailChangeAsync(string email, string token,string userId);
        Response<NoDataDto> UserNameChange(UserNameChangeDto userNameChangeDto);
        Task<Response<UserAppLikeDto>> GetUserByWhoLikesCommentAndPost(string userId);
        Response<UserApp> GetUserById(string userId);
        Task<Response<IEnumerable<UserSearchDto>>> GetUsersBySearch(string userName);
        Task<Response<NoDataDto>> AddProfilePhoto(IFormFile file,string userId);
        Task<Response<NoDataDto>> DeleteProfilePhoto(string userId);
        String GetUserProfilePath(string userId);
        Task<Response<IEnumerable<DataAnalysisDto>>> DataAnalysis();
        Response<UserNameAndPhotoPathDto> GetUserNameAndPhotoPathByUserId(string userId);
        Task<Response<NoDataDto>> AddAdminRoleToUser(string userId);
        Task<Response<NoDataDto>> AddDoctorRoleToUser(string userId);
    }
}