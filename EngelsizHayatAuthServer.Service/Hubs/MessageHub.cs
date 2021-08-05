using EngelsizHayatAuthServer.Core.Dtos.MessageDtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EngelsizHayatAuthServer.Core.Models;
using EngelsizHayatAuthServer.Core.Repositories;
using EngelsizHayatAuthServer.Core.UnitOfWork;
using Microsoft.AspNetCore.Http;

namespace EngelsizHayatAuthServer.Service.Hubs
{
    public class MessageHub : Hub
    {
        private IGenericRepository<SignalRConnectionString> _signalGenericRepository;
        private IUnitOfWork _unitOfWork;

        public MessageHub(IUnitOfWork unitOfWork, IGenericRepository<SignalRConnectionString> signalGenericRepository)
        {
            _unitOfWork = unitOfWork;
            _signalGenericRepository = signalGenericRepository;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            if (Context.User != null)
            {
                await InsertOrUpdateSignalRConnection(new SignalRConnectionString
                {
                    UserId = GetUserIdFromHttpContext(),
                    ConnectionId = connectionId,
                    Active = true
                });
            }
            await Clients.Others.SendAsync("userConnected", GetUserIdFromHttpContext());
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;

            if (Context.User != null)
            {
                await InsertOrUpdateSignalRConnection(new SignalRConnectionString
                {
                    UserId = GetUserIdFromHttpContext(),
                    ConnectionId = connectionId,
                    Active = false
                });
            }
            await Clients.Others.SendAsync("userDisconnected", GetUserIdFromHttpContext());
        }

        private async Task InsertOrUpdateSignalRConnection(SignalRConnectionString entity)
        {
            var isUserExist = _signalGenericRepository.Where(x => x.UserId == entity.UserId).Any();

            if (!isUserExist)
                await _signalGenericRepository.AddAsync(entity);
            else
                _signalGenericRepository.Update(entity);

            await _unitOfWork.CommitAsync();
        }

        private string GetUserIdFromHttpContext()
        {
            return Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}