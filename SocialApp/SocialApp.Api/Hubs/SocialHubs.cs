using Microsoft.AspNetCore.SignalR;
using SocialAppLibrary.Shared.IHub;

namespace SocialApp.Api.Hubs
{
    public class SocialHubs : Hub<ISocialHubClient>
    {
        public override Task OnConnectedAsync()
        {
            // Handle when a client connects
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            // Handle when a client disconnects
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string message)
        {
            // Handle when a client sends a message
           
        }
    }   
    
}
