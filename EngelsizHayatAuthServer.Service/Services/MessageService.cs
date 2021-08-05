using EngelsizHayatAuthServer.Core.Dtos.MessageDtos;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.Services;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using EngelsizHayatAuthServer.Service.Hubs;
using Microsoft.AspNetCore.SignalR;
using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Data;

namespace EngelsizHayatAuthServer.Service.Services
{
    public class MessageService : IMessageService
    {
        private readonly IHubContext<MessageHub> _messageHubContext;
        private IGenericRepository<Message> _messageGenericRepository;
        private readonly AppDbContext _context;
        private IGenericRepository<SignalRConnectionString> _signalGenericRepository;
        private IUnitOfWork _unitOfWork;
        private IUserService _userService;
        public MessageService(IHubContext<MessageHub> messageHubContext,
            IGenericRepository<Message> messageGenericRepository,
            IGenericRepository<SignalRConnectionString> signalGenericRepository, IUnitOfWork unitOfWork,
            AppDbContext context, IUserService userService)
        {
            _messageHubContext = messageHubContext;
            _messageGenericRepository = messageGenericRepository;
            _signalGenericRepository = signalGenericRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _userService = userService;
        }

        public async Task<Response<NoDataDto>> SendMessageAsync(SendMessageDto sendMessageDto)
        {
            var receiverClient = _signalGenericRepository.Where(x => x.UserId == sendMessageDto.ReceiverUserId).First();

            var senderClient = _signalGenericRepository.Where(x => x.UserId == sendMessageDto.SenderUserId).First();

            if (receiverClient != null)
            {
                if (sendMessageDto.ReceiverUserId == sendMessageDto.SenderUserId)
                {
                    await _messageHubContext.Clients.Clients(receiverClient.ConnectionId)
                        .SendAsync("errorMessage", "kendine mesaj atamazsın");
                    return Response<NoDataDto>.Fail("Kendine mesaj atamazsın!", 403, true);
                }

                if (string.IsNullOrEmpty(sendMessageDto.Message))
                {
                    await _messageHubContext.Clients.Clients(receiverClient.ConnectionId)
                        .SendAsync("errorMessage", "Boş mesaj atamazsın");
                    return Response<NoDataDto>.Fail("Kendine mesaj atamazsın!", 404, true);
                }

                await _messageGenericRepository.AddAsync(new Message
                {
                    Id = Guid.NewGuid().ToString(),
                    SenderId = sendMessageDto.SenderUserId,
                    ReceiverId = sendMessageDto.ReceiverUserId,
                    Text = sendMessageDto.Message,
                    Status = Convert.ToInt32(MessageStatusType.Sent),
                    SendDateUnix = DateTimeOffset.Now.ToUnixTimeSeconds()
                });
                await _unitOfWork.CommitAsync();
                await _messageHubContext.Clients.Client(receiverClient.ConnectionId)
                    .SendAsync("receiveMessage", sendMessageDto.Message);
                return Response<NoDataDto>.Success(204);
            }
            else
            {
                await _messageHubContext.Clients.Clients(senderClient.ConnectionId)
                    .SendAsync("errorMessage", "Mesaj atmak istediğiniz kullanıcı bulunamadı.");
                return Response<NoDataDto>.Fail("Kendine mesaj atamazsın!", 404, true);
            }
        }

        public async Task<Response<NoDataDto>> UpdateMessageStatusAsync(string messageId)
        {
            var currentMessage = await _messageGenericRepository.GetByIdAsync(messageId);

            if (currentMessage == null)
            {
                return Response<NoDataDto>.Fail("Mesaj bulunamadı! Lütfen daha sonra tekrar deneyiniz.", 404, true);
            }

            currentMessage.ReadDateUnix = DateTimeOffset.Now.ToUnixTimeSeconds();
            currentMessage.Status = Convert.ToInt32(MessageStatusType.Read);

            _messageGenericRepository.Update(currentMessage);
            await _unitOfWork.CommitAsync();

            return Response<NoDataDto>.Success(204);
        }

        public async Task<Response<IEnumerable<Message>>> GetMessageAsync(GetMessageDto getMessageDto)
        {
            var query = _messageGenericRepository.Where(x =>
                    x.ReceiverId == getMessageDto.ReceiverId && x.SenderId == getMessageDto.SenderId)
                .Union(_messageGenericRepository.Where(x =>
                    x.SenderId == getMessageDto.ReceiverId && x.ReceiverId == getMessageDto.SenderId))
                .OrderBy(x => x.SendDateUnix).AsEnumerable();

            var isUserOnline = _signalGenericRepository.Where(x => x.UserId == getMessageDto.SenderId).AsEnumerable()
                .Select(x => x.Active).First();

            if (isUserOnline)
            {
                await _messageHubContext.Clients.All.SendAsync("userConnected", getMessageDto.SenderId);
            }
            else
            {
                await _messageHubContext.Clients.All.SendAsync("userDisconnected", getMessageDto.SenderId);
            }

            return Response<IEnumerable<Message>>.Success(query, 200);
        }
        public async Task<Response<IEnumerable<MessageHistoryDto>>> GetMyMessageHistoryByUserId(string userId)
        {
            #region eski
            /*var result = from m in _context.Messages
                join u in _context.Users on m.SenderId equals u.Id
                where  (
                    from m2 in _context.Messages
                    where m2.ReceiverId == userId || m2.SenderId == userId
                    group m2 by (m2.SenderId == userId ? m2.ReceiverId : m2.SenderId)
                    into message
                    select message.Max(x => x.SendDateUnix)).Contains(m.SendDateUnix)
                         select new MessageHistoryDto
                {
                    UserName = u.UserName,
                    PhotoPath = u.PhotoPath,
                    MessageText = m.Text,
                    MessageDate = m.SendDateUnix.ToString()
                };
            */
            #endregion

            var resultMessageClientDtos = (from m in _context.Messages
                                           where (
                                               from m2 in _context.Messages
                                               where m2.ReceiverId == userId || m2.SenderId == userId
                                               group m2 by (m2.SenderId == userId ? m2.ReceiverId : m2.SenderId)
                                               into message
                                               select message.Max(x => x.SendDateUnix)).Contains(m.SendDateUnix)
                                           orderby m.SendDateUnix descending

                                           select new MessageClientDto
                                           {
                                               MessageId = m.Id,
                                               ReceiverId = m.ReceiverId,
                                               SenderId = m.SenderId
                                           }).ToList();

            if (!resultMessageClientDtos.Any())
            {
                return Response<IEnumerable<MessageHistoryDto>>.Fail("Kullanıcı mesaj listesi boş", 404, true);
            }

            var messageHistoryDtos = new List<MessageHistoryDto>();

            var resultMessageClientDtosNew = resultMessageClientDtos;

            foreach (var messageClientDto in resultMessageClientDtosNew)
            {
                if (messageClientDto.SenderId == userId)
                {
                    var messageHistoryDto = (from m in _context.Messages
                                             join u in _context.Users on m.ReceiverId equals u.Id
                                             where m.Id == messageClientDto.MessageId
                                             select new MessageHistoryDto
                                             {
                                                 UserId = u.Id,
                                                 UserName = u.UserName,
                                                 FirstName = u.FirstName,
                                                 LastName = u.LastName,
                                                 PhotoPath = u.PhotoPath,
                                                 MessageText = m.Text,
                                                 MessageDate = m.SendDateUnix.ToString()
                                             }).ToList();

                    var a = messageHistoryDto;

                    messageHistoryDtos.Add(a.First());
                }
                else
                {
                    var messageHistoryDto = (from m in _context.Messages
                                             join u in _context.Users on m.SenderId equals u.Id
                                             where m.Id == messageClientDto.MessageId
                                             select new MessageHistoryDto
                                             {
                                                 UserId = u.Id,
                                                 UserName = u.UserName,
                                                 FirstName = u.FirstName,
                                                 LastName = u.LastName,
                                                 PhotoPath = u.PhotoPath,
                                                 MessageText = m.Text,
                                                 MessageDate = m.SendDateUnix.ToString()
                           }).ToList();

                    var a = messageHistoryDto;

                    messageHistoryDtos.Add(a.First());
                }
            } 

            var messageHistoryDtosNew = messageHistoryDtos;

            foreach (var messageHistoryDto in messageHistoryDtosNew)
            {
                messageHistoryDto.MessageDate = getTimeInterval(messageHistoryDto.MessageDate);
            }

            return Response<IEnumerable<MessageHistoryDto>>.Success(messageHistoryDtosNew, 200);
        }

        private string getTimeInterval(string time)
        {
            var timeInterval = "";

            var timeDifference = (DateTimeOffset.Now.ToUnixTimeSeconds() - long.Parse(time));

            DateTime result = UnixTimestampToDateTime(long.Parse(time));

            if (timeDifference < 86400)
                timeInterval = result.ToShortTimeString();
            else if (timeDifference > 86400 && timeDifference < 86400 * 2)
                timeInterval = "Dün";
            else if (timeDifference > 86400 * 2 && timeDifference < 2592000)
                timeInterval = result.ToShortDateString();

            return timeInterval;
        }

        public DateTime UnixTimestampToDateTime(long unixTime)
        {
            System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
            return dtDateTime;
        }
    }
}