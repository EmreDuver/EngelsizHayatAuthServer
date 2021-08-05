using System.Collections.Generic;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Dtos.MessageDtos;
using EngelsizHayatAuthServer.Core.Models;
using SharedLibrary.Dtos;

namespace EngelsizHayatAuthServer.Core.Services
{
    public interface IMessageService
    {
        Task<Response<NoDataDto>> SendMessageAsync(SendMessageDto sendMessageDto);
        Task<Response<NoDataDto>> UpdateMessageStatusAsync(string messageId);
        Task<Response<IEnumerable<Message>>> GetMessageAsync(GetMessageDto getMessageDto);
        Task<Response<IEnumerable<MessageHistoryDto>>> GetMyMessageHistoryByUserId(string userId);
    }
}
