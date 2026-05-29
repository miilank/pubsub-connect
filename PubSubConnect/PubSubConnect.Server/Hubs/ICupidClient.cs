using PubSubConnect.Server.Models;

namespace PubSubConnect.Server.Hubs
{
    public interface ICupidClient
    {
        Task ReceiveLetter(Letter letter);
    }
}
