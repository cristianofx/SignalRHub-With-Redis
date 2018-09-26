using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalRHub.Contracts;
using SignalRHub.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRHub.SignalR
{
    public class SignalRMainHub : Hub, ISignalRMainHub
    {
        private HubSubscriptionInRedis _hubInfoInRedis;
        protected IHubContext<SignalRMainHub> _context;

        public SignalRMainHub(IHubContext<SignalRMainHub> context, HubSubscriptionInRedis hubInfoInRedis)
        {
            _context = context;
            _hubInfoInRedis = hubInfoInRedis;
        }

        public IEnumerable<string> GetAllConnectionIds()
        {
            return _hubInfoInRedis.GetAllConnections().Select(x => $"{x.Process} : {x.ConnectionId}");
        }

        public void EchoAll(string message)
        {
            _context.Clients.All.SendAsync("SendAll", message);
        }

        public void EchoClient(string message)
        {
            _context.Clients.Client("connectionId").SendAsync(message);
        }

        public void NotifyAllSubscribed(string process, object message, string userId = null)
        {
            var subscribed = userId == null ? _hubInfoInRedis.GetAllUsersByProcess(process) : _hubInfoInRedis.GetAllUsersByProcess(process).Where(f => f.UserId == userId);

            foreach (var client in subscribed)
            {
                _context.Clients.Client(client.ConnectionId).SendAsync(process, JsonConvert.SerializeObject(message));
            }
        }

        public async Task Subscribe(string process, string userId)
        {
            await Task.Run(() =>
            {
                _hubInfoInRedis.Subscribe(process, userId, Context.ConnectionId);
            });
        }

        public async Task Unsubscribe(string process, string userId)
        {
            await Task.Run(() =>
            {
                _hubInfoInRedis.RemoveSubscription(process, Context.ConnectionId);
            });
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _hubInfoInRedis.RemoveAllSubscriptionFromClient(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
