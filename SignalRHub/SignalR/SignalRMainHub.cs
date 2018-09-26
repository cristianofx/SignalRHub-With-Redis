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
        private HubSubscriptionInMemory _hubInfoInMemory;
        protected IHubContext<SignalRMainHub> _context;

        public SignalRMainHub(IHubContext<SignalRMainHub> context, HubSubscriptionInMemory hubInfoInMemory)
        {
            _context = context;
            _hubInfoInMemory = hubInfoInMemory;
        }

        public IEnumerable<string> GetAllConnectionIds()
        {
            return _hubInfoInMemory.GetAllConnections().Select(x => $"{x.Process} : {x.ConnectionId}");
        }

        //you're going to invoke this method from the client app
        public void EchoAll(string message)
        {
            _context.Clients.All.SendAsync("SendAll", message);
        }

        public void EchoClient(string message)
        {
            //you're going to configure your client app to listen for this
            _context.Clients.Client("connectionId").SendAsync(message);
        }

        public void NotifyAllSubscribed(string process, object message, string userId = null)
        {
            var subscribed = userId == null ? _hubInfoInMemory.GetAllUsersByProcess(process) : _hubInfoInMemory.GetAllUsersByProcess(process).Where(f => f.UserId == userId);

            //you're going to configure your client app to listen for this
            foreach (var client in subscribed)
            {
                _context.Clients.Client(client.ConnectionId).SendAsync(process, JsonConvert.SerializeObject(message));
            }
        }

        public async Task Subscribe(string process, string userId)
        {
            await Task.Run(() =>
            {
                _hubInfoInMemory.Subscribe(process, userId, Context.ConnectionId);
            });
        }

        public async Task Unsubscribe(string process, string userId)
        {
            await Task.Run(() =>
            {
                _hubInfoInMemory.RemoveSubscription(process, Context.ConnectionId);
            });
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _hubInfoInMemory.RemoveAllSubscriptionFromClient(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
