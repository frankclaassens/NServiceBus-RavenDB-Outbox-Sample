using System;
using NServiceBus;
using Orders.Commands;

namespace Console.App
{
	class Program
	{
		static void Main(string[] args)
		{
			var busConfiguration = new BusConfiguration();
			busConfiguration.EndpointName("Console.App.Endpoint");
			busConfiguration.EnableInstallers();
			busConfiguration.UsePersistence<InMemoryPersistence>();

			using (var bus = Bus.Create(busConfiguration).Start())
			{
				System.Console.WriteLine("Press 'enter' to send a CreateOrder messages");
				System.Console.WriteLine("Press any other key to exit");

				var orderId = Guid.NewGuid();
				var startOrder = new CreateOrder
				{
					Id = orderId,
					CustomerName = "Frank Claassens"
				};
				bus.Send(startOrder);
			}
		}
	}
}
