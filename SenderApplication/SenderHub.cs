using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SenderApplication
{
    public class SenderHub : Hub
    {
        public async Task SendMessage (Guid userId)
        {
            await Clients.All.SendAsync("Receive Message User Id", userId);
        }
    }
}
