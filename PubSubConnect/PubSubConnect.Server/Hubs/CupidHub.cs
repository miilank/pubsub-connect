using Microsoft.AspNetCore.SignalR;
using PubSubConnect.Server.Services;

namespace PubSubConnect.Server.Hubs
{
    public class CupidHub : Hub
    {
        private readonly ICupidService _cupidService;

        public CupidHub(ICupidService cupidService)
        {
            _cupidService = cupidService;
        }

        public Task InitSinglePerson(string username, string city, int age, string phone)
        {
            _cupidService.RegisterPerson(Context.ConnectionId, username, city, age, phone);
            return Task.CompletedTask;
        }

        public Task ConfirmReceipt()
        {
            _cupidService.ConfirmReceipt(Context.ConnectionId);
            return Task.CompletedTask;
        }

        public Task BlockUser(string username)
        {
            _cupidService.BlockUser(Context.ConnectionId, username);
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _cupidService.RemovePerson(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
