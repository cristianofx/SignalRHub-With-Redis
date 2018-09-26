using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SignalRHub.Contracts
{
    public interface ISignalRMainHub
    {
        void EchoAll(string message);

        void EchoClient(string message);

        void NotifyAllSubscribed(string process, object message, string userId = null);


        Task Subscribe(string process, string userId);

        Task OnConnectedAsync();

        Task OnDisconnectedAsync(Exception exception);

        IEnumerable<string> GetAllConnectionIds();
    }
}
