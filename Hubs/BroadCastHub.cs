using Microsoft.AspNetCore.SignalR;

namespace KingUploader.Hubs
{
    public class BroadCastHub : Hub
    {
        public Task Feedback(string message)
        {
            return Clients.Caller.SendAsync("feedBack", message);
        }
    }
}
