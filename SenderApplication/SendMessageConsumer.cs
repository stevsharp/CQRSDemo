using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SenderApplication
{
    public class SendMessageConsumer : IConsumer<Message>
    {

        private IHubContext<SenderHub> _hub;

        public SendMessageConsumer(IHubContext<SenderHub> hub)
        {
            _hub = hub;
        }

        public async Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Receive message value: {context.Message.Value}");

            await _hub.Clients.All.SendAsync($"Receive message value: {context.Message.Value}");

            // return Task.CompletedTask;
        }
    }
}
