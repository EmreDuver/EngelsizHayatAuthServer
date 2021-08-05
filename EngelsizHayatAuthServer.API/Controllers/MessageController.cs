using EngelsizHayatAuthServer.Core.Dtos.MessageDtos;
using EngelsizHayatAuthServer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Services;

namespace EngelsizHayatAuthServer.API.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : CustomBaseController
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost("sendmessage")]
        public async Task<IActionResult> SendMessage(SendMessageDto sendMessageDto)
        {
            return ActionResultInstance(await _messageService.SendMessageAsync(sendMessageDto));
        }
        [HttpPost("getmessage")]
        public async Task<IActionResult> TakeAccessToken(GetMessageDto getMessageDto)
        {
            return ActionResultInstance(await _messageService.GetMessageAsync(getMessageDto));
        }
        [HttpPost("updatemessagestatusasync")]
        public async Task<IActionResult> UpdateMessageStatusAsync([FromBody] string messageId)
        {
            return ActionResultInstance(await _messageService.UpdateMessageStatusAsync(messageId));
        }

        [HttpGet("getmymessageshistory/{userId}")]
        public async Task<IActionResult> GetMyMessageHistoryByUserId(string userId)
        {
            return ActionResultInstance(await _messageService.GetMyMessageHistoryByUserId(userId));
        }
    }
}