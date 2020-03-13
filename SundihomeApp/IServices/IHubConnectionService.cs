using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using SundihomeApi.Entities.Mongodb;

namespace SundihomeApp.IServices
{
    public interface IHubConnectionService
    {
        HubConnection Hub { get; set; }
        bool IsConnect { get; }
        Task Start(string connectionUrl, Action<MessageItem> OnNewMessage);
        void RegisterNewMessage(Action<MessageItem> OnNewMessage);
        Task ReStart();
        Task UpdateChatUserId(string UserId);
        Task Stop();
    }
}
