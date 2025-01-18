using Microsoft.AspNetCore.SignalR;

namespace BL.Hubs;

    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Client {Context.ConnectionId} connected");
            Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
            return base.OnConnectedAsync();
        }
    }
