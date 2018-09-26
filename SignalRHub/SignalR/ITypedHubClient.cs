using System.Threading.Tasks;

namespace SignalRHub.SignalR
{
    public interface ITypedHubClient
    {
        Task BroadcastMessage(string type, string payload);
    }
}
