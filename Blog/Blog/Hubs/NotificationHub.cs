using Blog.ViewDTO;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Blog.Hubs
{
    public class NotificationHub : Hub
    {
        public string GetConnectionNotiId()
        {
            return Context.ConnectionId;
        }
        public override async Task OnConnectedAsync()
        {
            if (!ConnectedUser.IdList.Contains(Context.User.Identity.Name))
                ConnectedUser.IdList.Add(Context.User.Identity.Name);
            string json = JsonSerializer.Serialize(ConnectedUser.IdList);

            await Clients.All.SendAsync("UserJoined", json);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (ConnectedUser.IdList.Contains(Context.User.Identity.Name))
                ConnectedUser.IdList.Remove(Context.User.Identity.Name);
            string json = JsonSerializer.Serialize(ConnectedUser.IdList);

            await Clients.All.SendAsync("UserJoined", json);
        }
    }
}
