using System;
using Microsoft.AspNetCore.SignalR.Client;
using SundihomeApp.IServices;
using System.Threading.Tasks;
using SundihomeApi.Entities.Mongodb;

namespace SundihomeApp.Services
{
    public class HubConnectionService : IHubConnectionService
    {
        public HubConnection Hub { get; set; }
        public bool IsConnect => this.Hub != null && this.Hub.State == HubConnectionState.Connected;

        public async Task Start(string connectionUrl, Action<MessageItem> OnNewMessage)
        {
            this.Hub = new HubConnectionBuilder().WithUrl(connectionUrl).Build();
            RegisterNewMessage(OnNewMessage);
            await Hub.StartAsync();
        }

        public void RegisterNewMessage(Action<MessageItem> OnNewMessage)
        {
            this.Hub.Remove("NewMessage");
            this.Hub.On("NewMessage", OnNewMessage);
        }

        public async Task ReStart()
        {
            try
            {
                if (this.Hub != null && this.Hub.State == HubConnectionState.Disconnected)
                {
                    await Hub.StartAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }


        public async Task UpdateChatUserId(string UserId)
        {
            if (IsConnect)
            {
                await Hub.InvokeAsync("UpdateChatUserId", UserId);
            }
        }

        public async Task Stop()
        {
            if (IsConnect)
            {
                await Hub.StopAsync();
            }
        }
    }
}
