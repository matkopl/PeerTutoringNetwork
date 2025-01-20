using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using BL.Hubs;

namespace BL.Services;

public interface IChatService
{
    Task SendMessage(string user, string message);
}

public class ChatService : IChatService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public ChatService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessage(string user, string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}