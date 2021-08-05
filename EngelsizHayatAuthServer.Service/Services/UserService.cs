using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Dtos.UserAppDtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;
using SharedLibrary.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using EngelsizHayatAuthServer.Core.Dtos.BigData;
using Microsoft.EntityFrameworkCore;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserApp> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public UserService(UserManager<UserApp> userManager, IHttpContextAccessor httpContextAccessor, AppDbContext context2)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context2;
        }

        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName,
                FirstName = createUserDto.Name,
                LastName = createUserDto.SurName
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description).ToList();
                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }

            SendEmailForConfirmation(user.Id);

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(user), 200);
        }

        public async Task<Response<NoDataDto>> AddAdminRoleToUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Response<NoDataDto>.Fail("Kullanıcı bulunamadı", 400, true);
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");

            if (!result.Succeeded)
            {
                return Response<NoDataDto>.Fail("Rol bulunamadı", 400, true);
            }

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> AddDoctorRoleToUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Response<NoDataDto>.Fail("Kullanıcı bulunamadı", 400, true);
            }

            var result = await _userManager.AddToRoleAsync(user, "Doctor");

            if (!result.Succeeded)
            {
                return Response<NoDataDto>.Fail("Rol bulunamadı", 400, true);
            }

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<UserAppDto>> GetUserByNameAsync(string userIdFromHttpContext)
        {
            var user = await _userManager.FindByIdAsync(userIdFromHttpContext);
            UserAppDto userAppDto = null;
            if (user == null)
            {
                return Response<UserAppDto>.Fail("Username not found", 404, true);
            }

            // Location belirtilmedi default
            var result = from u in _context.Users
                         join g in _context.Genders on u.GenderId equals g.Id
                         where u.Id == user.Id
                         select new UserAppDto
                         {
                             Id = u.Id,
                             UserName = u.UserName,
                             Email = u.Email,
                             BirthDay = u.BirthDay,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             GenderId = g.Id,
                             PhotoPath = u.PhotoPath,
                             Biography = u.Biography
                         };

            var x = result.ToList();
            userAppDto = result.FirstOrDefault();

            if (userAppDto == null)
            {
                return Response<UserAppDto>.Fail("not found", 404, true);
            }

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(userAppDto), 200);
        }

        public async Task<Response<UserAppDto>> GetUserByIdForOtherUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            UserAppDto userAppDto = null;

            if (user == null)
            {
                return Response<UserAppDto>.Fail("Kullanıcı bulunamadı", 404, true);
            }

            var result = from u in _context.Users
                join g in _context.Genders on u.GenderId equals g.Id
                where u.Id == user.Id
                select new UserAppDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    BirthDay = u.BirthDay,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    GenderId = g.Id,
                    PhotoPath = u.PhotoPath,
                    Biography = u.Biography
                };

            var x = result.ToList();
            userAppDto = result.FirstOrDefault();

            if (userAppDto == null)
            {
                return Response<UserAppDto>.Fail("Kullanıcı bilgileri alınamadı.", 404, true);
            }

            return Response<UserAppDto>.Success(ObjectMapper.Mapper.Map<UserAppDto>(userAppDto), 200);
        }

        public async Task<Response<NoDataDto>> VerifyEmailAsync(string userId, string token)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            //var confirmToken = token.Replace("%2F", "/");
            var confirmToken = HttpUtility.UrlDecode(token).Replace(" ", "+");

            if (user == null || String.IsNullOrEmpty(confirmToken))
                return Response<NoDataDto>.Fail("https://kullanici.engelsizbirey.com/mail_verified_error1.html", 404, true);

            if (await _userManager.IsEmailConfirmedAsync(user))
                return Response<NoDataDto>.Fail("https://kullanici.engelsizbirey.com/mail_verified_error2.html", 404, true);

            var isTokenValid = await _userManager.ConfirmEmailAsync(user, confirmToken);

            if (!isTokenValid.Succeeded)
            {
                SendEmailForConfirmation(user.Id);
                return Response<NoDataDto>.Fail("https://kullanici.engelsizbirey.com/mail_verified_error3.html", 404, true);
            }


            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<NoDataDto>> ResetPasswordRequestAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return Response<NoDataDto>.Fail("Username not found", 404, true);

            SendEmailForResetPassword(user.Email);

            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<NoDataDto>> UpdateProfileAsync(UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(updateUserDto.Id);

            if (user == null)
            {
                return Response<NoDataDto>.Fail("Username not found", 404, true);
            }

            if (updateUserDto.UserName != user.UserName)
            {
                var changeUserName = await _userManager.SetUserNameAsync(user, updateUserDto.UserName);

                if (!changeUserName.Succeeded)
                {
                    return Response<NoDataDto>.Fail("Kullanıcı adı kullanımda lütfen başka bir kullanıcı adı deneyin.", 404, true);
                }
            }

            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.GenderId = updateUserDto.GenderId;
            user.BirthDay = updateUserDto.BirthDay;
            user.Biography = updateUserDto.Biography;

            var result = _userManager.UpdateAsync(user).Result;

            if (!result.Succeeded) return Response<NoDataDto>.Fail("Lütfen girdiğiniz değerleri kontrol ediniz.", 404, true);

            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<NoDataDto>> EmailChangeRequestAsync(EmailChangeDto emailChangeDto)
        {
            var user = await _userManager.FindByIdAsync(emailChangeDto.Id);

            if (user == null)
                return Response<NoDataDto>.Fail("Kullanıcı bulunamadı.", 404, true);

            if (await _userManager.FindByEmailAsync(emailChangeDto.Email) != null)
            {
                return Response<NoDataDto>.Fail("Email kullanımda!", 404, true);
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, emailChangeDto.Email);

            SendEmailForEmailChange(user.Id, emailChangeDto.Email, token);
            return Response<NoDataDto>.Success(200);
        }

        public async Task<Response<NoDataDto>> EmailChangeAsync(string email, string token, string userId)
        {
            string decodedUserId = HttpUtility.UrlDecode(userId);
            var user = _userManager.FindByIdAsync(decodedUserId).Result;
            string oldMail = user.Email;

            if (user == null || String.IsNullOrEmpty(token))
                return Response<NoDataDto>.Fail("Username not found", 404, true);

            string decodedConfirmToken = HttpUtility.UrlDecode(token).Replace(" ", "+");
            string decodedEmail = HttpUtility.UrlDecode(email);

            var changeEmailAsync = await _userManager.ChangeEmailAsync(user, decodedEmail, decodedConfirmToken);

            if (!changeEmailAsync.Succeeded)
                Response<NoDataDto>.Fail("Eposta değiştirilemedi", 404, true);

            EmailHelper.SendEmailForOldAndNewEmail(oldMail, email);

            var cancelEmailChangeToken = await _userManager.GenerateChangeEmailTokenAsync(user, oldMail);

            return Response<NoDataDto>.Success(200);
        }

        public Response<NoDataDto> UserNameChange(UserNameChangeDto userNameChangeDto)
        {
            var user = _userManager.FindByIdAsync(userNameChangeDto.Id).Result;

            if (user == null)
                return Response<NoDataDto>.Fail("Kullanıcı bulunamadı.", 404, true);

            user.UserName = userNameChangeDto.UserName;

            var result = _userManager.UpdateAsync(user).Result;

            if (!result.Succeeded)
                return Response<NoDataDto>.Fail("Kullanıcı adı kullanılıyor.", 404, true);

            return Response<NoDataDto>.Success(200);
        }

        public Response<UserApp> GetUserById(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return Response<UserApp>.Fail("Kullanıcı bulunamadı.", 404, true);
            }

            return Response<UserApp>.Success(user, 200);
        }

        public async Task<Response<IEnumerable<UserSearchDto>>> GetUsersBySearch(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return Response<IEnumerable<UserSearchDto>>.Fail("Lütfen aramak istediğiniz kullanıcın, kullanıcı adını yazınız.", 404, true);
            }
            var userAppList = await _userManager.Users.Where(x => x.UserName.Contains(userName)).ToListAsync();

            var userList = new List<UserSearchDto>();

            foreach (var userApp in userAppList)
            {
                userList.Add(ObjectMapper.Mapper.Map<UserSearchDto>(userApp));
            }

            return userList.Count == 0
                ? Response<IEnumerable<UserSearchDto>>.Fail("Aradığınız isimde bir kullanıcı bulunamadı", 404, true)
                : Response<IEnumerable<UserSearchDto>>.Success(userList, 200);
        }

        public async Task<Response<NoDataDto>> AddProfilePhoto(IFormFile file, string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return Response<NoDataDto>.Fail("Kullanıcı bulunamadı.", 404, true);
            }

            if (file == null)
            {
                return Response<NoDataDto>.Fail("Eklemek istediğiniz resim bulunamadı.", 404, true);
            }

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", user.PhotoPath);

            var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", userId + Path.GetExtension(file.FileName));

            if (!(Path.GetExtension(file.FileName) == ".jpg" || Path.GetExtension(file.FileName) == ".jpeg" || Path.GetExtension(file.FileName) == ".png"))
            {
                return Response<NoDataDto>.Fail("Lütfen geçerli bir dosya türü giriniz.", 404, true);
            }

            if (!oldPath.Contains("default_user_photo.jpg"))
            {
                File.Delete(oldPath);
            }

            await using (var stream = new FileStream(newPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            user.PhotoPath = user.Id + Path.GetExtension(file.FileName);
            await _userManager.UpdateAsync(user);

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<NoDataDto>> DeleteProfilePhoto(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            if (user == null)
            {
                return Response<NoDataDto>.Fail("Kullanıcı bulunamadı.", 404, true);
            }

            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", user.PhotoPath);

            if (!oldPath.Contains("default_user_photo.jpg"))
            {
                File.Delete(oldPath);
            }

            user.PhotoPath = "default_user_photo.jpg";
            await _userManager.UpdateAsync(user);

            return Response<NoDataDto>.Success(204);
        }

        public String GetUserProfilePath(string userId)
        {
            return _userManager.FindByIdAsync(userId).Result.PhotoPath;
        }

        public async Task<Response<IEnumerable<DataAnalysisDto>>> DataAnalysis()
        {
            var userList = await _userManager.Users.ToListAsync();
            var dataAnalysisDto = (from userApp in userList let timeStamp = new DateTimeOffset(userApp.BirthDay, TimeSpan.Zero).ToUnixTimeSeconds() select new DataAnalysisDto {GenderId = userApp.GenderId, BirthDay = timeStamp}).ToList();

            var newPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", DateTimeOffset.Now.ToUnixTimeSeconds()+"dataAgeAndGender.json");
            var backupPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "dataAgeAndGender.json");

            if (File.Exists(newPath))
            {
                File.Delete(newPath);
            }

            var json = JsonSerializer.Serialize(dataAnalysisDto);
            await File.WriteAllTextAsync(newPath, json);
            await File.WriteAllTextAsync(backupPath, json);

            return Response<IEnumerable<DataAnalysisDto>>.Success(dataAnalysisDto, 200);

        }

        public Response<UserNameAndPhotoPathDto> GetUserNameAndPhotoPathByUserId(string userId)
        {
            var user = _userManager.Users.First(x => x.Id == userId);
            var userNameAndPhotoPath = ObjectMapper.Mapper.Map<UserNameAndPhotoPathDto>(user);

            return Response<UserNameAndPhotoPathDto>.Success(userNameAndPhotoPath, 200);
        }

        public async Task<Response<UserAppLikeDto>> GetUserByWhoLikesCommentAndPost(string userId)
        {
            var user = GetUserById(userId);

            if (!user.IsSuccessful)
                return Response<UserAppLikeDto>.Fail(user.Error.Errors.ToString(), 404, true);

            var data = ObjectMapper.Mapper.Map<UserAppLikeDto>(user.Data);

            return Response<UserAppLikeDto>.Success(data, 200);
        }

        public async Task<Response<NoDataDto>> ResetPasswordAsync(string userId, string token)
        {
            var user = _userManager.FindByIdAsync(userId).Result;
            var confirmToken = token.Replace("%2F", "/");

            if (user == null || String.IsNullOrEmpty(confirmToken))
                return Response<NoDataDto>.Fail("Username not found", 404, true);

            var newPassword = Guid.NewGuid().ToString().Substring(0, 8);

            var isTokenValid = await _userManager.ResetPasswordAsync(user, confirmToken, newPassword);

            if (!isTokenValid.Succeeded)
                Response<NoDataDto>.Fail("Token invalid", 404, true);

            EmailHelper.SendEmailForPassword(user.Email, newPassword);

            return Response<NoDataDto>.Success(200);
        }

        private void SendEmailForEmailChange(string userId, string mail, string token)
        {
            /*var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                return;
            }*/

            //var confirmToken = token.Replace("/", "%2F");

            var linkHost = _httpContextAccessor.HttpContext.Request.Host.ToString();
            var oldLinkSubClient = _httpContextAccessor.HttpContext.Request.Path.ToString(); //api/user/createuser
            var newLinkSubClient = oldLinkSubClient.Replace("emailchangerequest", "changeemail");

            var linkWithoutParameter = linkHost + newLinkSubClient + "/";

            string encodedConfirmToken = HttpUtility.UrlEncode(token);
            string encodedEmail = HttpUtility.UrlEncode(mail);
            string encodedUserId = HttpUtility.UrlEncode(userId);

            var link = $"{linkWithoutParameter}email={encodedEmail}&userId={encodedUserId}&token={encodedConfirmToken}";

            EmailHelper.SendEmailForEmailChange(link, mail);
        }

        private void SendEmailForConfirmation(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            //var token = _userManager.GenerateEmailConfirmationTokenAsync(user).Result.Replace("/", "%2F");
            var token = HttpUtility.UrlEncode(_userManager.GenerateEmailConfirmationTokenAsync(user).Result);

            var linkHost = _httpContextAccessor.HttpContext.Request.Host.ToString();
            var oldLinkSubClient = _httpContextAccessor.HttpContext.Request.Path.ToString(); //api/user/createuser
            var newLinkSubClient = oldLinkSubClient.ToLower().Replace("createuser", "verifyemail");

            var linkWithoutParamater = linkHost + newLinkSubClient + "/";

            var link = $"{linkWithoutParamater}userId={userId}&token={token}";

            EmailHelper.SendEmailForConfirmation(link: link, user.Email);
        }

        private void SendEmailForResetPassword(string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result.Replace("/", "%2F");

            var linkHost = _httpContextAccessor.HttpContext.Request.Host.ToString();
            var oldLinkSubClient = _httpContextAccessor.HttpContext.Request.Path.ToString();
            var newLinkSubClient = oldLinkSubClient.Replace("resetpasswordrequest", "resetpassword");

            var linkWithoutParamater = linkHost + newLinkSubClient + "/";

            var link = $"{linkWithoutParamater}userId={user.Id}&token={token}";

            EmailHelper.SendEmailForResetPassword(link, email);

            EmailHelper.SendEmailForResetPassword(link, user.Email);
        }
    }
}