using System.Security.Claims;
using EngelsizHayatAuthServer.Core.Dtos;
using EngelsizHayatAuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EngelsizHayatAuthServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : CustomBaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            return ActionResultInstance(await _userService.CreateUserAsync(createUserDto));
        }

        [HttpGet("verifyemail/userId={userId}&token={token}")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var verify = await _userService.VerifyEmailAsync(userId, token);
            ActionResultInstance(verify);
            return verify.IsSuccessful ? Redirect("https://kullanici.engelsizbirey.com/mail_verified_successfully.html") : Redirect(verify.Error.Errors[0].ToString());
        }

        [HttpPost("resetpasswordrequest")]
        public async Task<IActionResult> ResetPasswordRequest([FromBody]string email)
        {
            return ActionResultInstance(await _userService.ResetPasswordRequestAsync(email));
        }

        [HttpGet("resetpassword/userId={userId}&token={token}")]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            return ActionResultInstance(await _userService.ResetPasswordAsync(userId, token));
        }
       
        [Authorize]
        [HttpPost("updateprofile")]
        public async Task<IActionResult> UpdateProfile(UpdateUserDto updateUserDto)
        {
            return ActionResultInstance(await _userService.UpdateProfileAsync(updateUserDto));
        }
        [Authorize]
        [HttpPost("changeusername")]
        public  IActionResult ChangeUserName(UserNameChangeDto userNameChangeDto)
        {
            return ActionResultInstance(_userService.UserNameChange(userNameChangeDto));
        }

        [Authorize]
        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser()
        {
            return ActionResultInstance(await _userService.GetUserByNameAsync(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
        [Authorize]
        [HttpPost("getotheruser")]
        public async Task<IActionResult> GetOtherUser([FromBody] string userId)
        {
            return ActionResultInstance(await _userService.GetUserByIdForOtherUserProfileAsync(userId));
        }

        [Authorize]
        [HttpPost("getusersbysearch")]
        public async Task<IActionResult> GetUsersBySearch([FromBody] string userName)
        {
            return ActionResultInstance( await _userService.GetUsersBySearch(userName));
        }

        [Authorize]
        [HttpPost("emailchangerequest")]
        public async Task<IActionResult> EmailChangeRequest(EmailChangeDto emailChangeDto)
        {
            return ActionResultInstance(await _userService.EmailChangeRequestAsync(emailChangeDto));
        }

        [HttpGet("changeemail/email={email}&userId={userId}&token={token}")]
        public async Task<IActionResult> SendEmailChange(string email, string userId,string token)
        {
            return ActionResultInstance(await _userService.EmailChangeAsync(email,token,userId));
        }

        [Authorize]
        [HttpPost("adduserimage")]
        public async Task<IActionResult> AddUserImage(IFormFile file,[FromForm]string userId)
        {
            return ActionResultInstance(await _userService.AddProfilePhoto(file,userId));
        }
        [Authorize]
        [HttpPost("deleteuserimage")]
        public async Task<IActionResult> DeleteUserImage([FromBody]string userId)
        {
            return ActionResultInstance(await _userService.DeleteProfilePhoto(userId));
        }

        [Authorize]
        [HttpGet("dataanalysis")]
        public async Task<IActionResult> DataAnalysis()
        {
            return ActionResultInstance(await _userService.DataAnalysis());
        }

        [Authorize]
        [HttpGet("getusernameandphotopath/{userId}")]
        public IActionResult GetUserNameAndPhotoPath(string userId)
        {
            return ActionResultInstance( _userService.GetUserNameAndPhotoPathByUserId(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("addadminroletouser")]
        public async Task<IActionResult> AddAdminRoleToUser([FromBody] string userId)
        {
            return ActionResultInstance(await _userService.AddAdminRoleToUser(userId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("adddoctorroletouser")]
        public async Task<IActionResult> AddDoctorRoleToUser([FromBody] string userId)
        {
            return ActionResultInstance(await _userService.AddDoctorRoleToUser(userId));
        }

    }
}