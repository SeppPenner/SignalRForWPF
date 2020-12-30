using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRForWPF.Shared;

namespace SignalRForWPF.Server
{
    public class TestHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            Console.WriteLine($"Received message {message.MessageText} from client {message.User}");
            await this.Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}