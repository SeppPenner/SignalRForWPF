using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using SignalRForWPF.Shared;

namespace SignalRForWPF.Server
{
    public class TestHub : Hub
    {
        private readonly ILogger Log = new DebugLogger("TestHub");

        public async Task SendMessage(Message message)
        {
            this.Log.LogInformation($"Received message {message.MessageText} from client {message.User}");
            await this.Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}