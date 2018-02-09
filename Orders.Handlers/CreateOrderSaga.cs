using System;
using Common;
using NServiceBus;
using NServiceBus.Logging;
using NServiceBus.RavenDB.Persistence;
using NServiceBus.Saga;
using Orders.Commands;
using Orders.Events;
using Shipping.Commands;

namespace Orders.Handlers
{
	public class CreateOrderSaga : Saga<OrderSagaData>,
		IAmStartedByMessages<CreateOrder>,
		IHandleTimeouts<CompleteOrder>
	{
		private readonly IBus _bus;

		private readonly ISessionProvider _sessionProvider;

		public CreateOrderSaga(IBus bus, ISessionProvider sessionProvider)
		{
			_bus = bus;
			_sessionProvider = sessionProvider;
		}

		protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderSagaData> mapper)
		{
			mapper.ConfigureMapping<CreateOrder>(message => message.Id)
				.ToSaga(sagaData => sagaData.OrderId);
		}

		public void Handle(CreateOrder message)
		{
			_sessionProvider.Session.Store(message);                            // SAVE NEW ORDER DOCUMENT

			Data.OrderId = message.Id;
			var orderDescription = $"The saga for order {message.Id}";
			Data.OrderDescription = orderDescription;

			Console.WriteLine($"Received CreateOrder message {Data.OrderId}. Starting Saga");

			//Bus.Publish(new OrderCreated { OrderId = Data.OrderId });             

			var shipOrder = new CreateShippingList                          
			{
				OrderId = message.Id,
			};
			((Saga)this).Bus.Send(shipOrder);                               // CREATE ORDER SHIPPING LIST DOCUMENT          
			Console.WriteLine($"Sent Create Shipping List message for order {Data.OrderId}");

			Console.WriteLine("Order will complete in 5 seconds");
			var timeoutData = new CompleteOrder
			{
				OrderDescription = Data.OrderDescription
			};
			RequestTimeout(TimeSpan.FromSeconds(5), timeoutData);
		}

		public void Timeout(CompleteOrder state)
		{
			Console.WriteLine($"Saga with OrderId {Data.OrderId} completed");

			var orderCompleted = new OrderCompleted
			{
				OrderId = Data.OrderId
			};
			_bus.Publish(orderCompleted);

			MarkAsComplete();
		}
	}
}