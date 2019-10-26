using MassTransit;
using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.ReceiveEndpoint(host, "test_queue", ep =>
                {
                    ep.Handler<YourMessage>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            bus.Start(); // This is important!

            bus.Publish(new YourMessage { Text = "Hi" });

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            bus.Stop();

            Console.ReadLine();
        }
    }

    public class YourMessage
    {
        public string Text { get; set; }
    }
}
