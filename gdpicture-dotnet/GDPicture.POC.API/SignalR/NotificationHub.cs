using Microsoft.AspNetCore.SignalR;

namespace GDPicture.POC.API.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendMessageAsync(NotificationMessage message)
        {
            await Clients.All.SendAsync("ConversionCompleted", message);
        }
    }
}
