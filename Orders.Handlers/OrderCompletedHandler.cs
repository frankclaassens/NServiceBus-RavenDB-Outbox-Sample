using System;
using Common;
using NServiceBus;
using Orders.Commands;
using Orders.Events;
using NServiceBus.RavenDB.Persistence;
using Raven.Client;

namespace Orders.Handlers
{
	public class CreateOrderHandler : MessageHandlerBase<OrderCompleted>
	{
		//private readonly ISessionProvider _session;
		//private readonly IDocumentSession _session;

		public CreateOrderHandler()
		{
		}

		protected override void HandleImpl(OrderCompleted message)
		{
			var order = RavenSession.Session.Load<CreateOrder>(message.OrderId); // MARK ORDER AS COMPLETED
			order.OrderStatus = OrderStatus.Completed;
			RavenSession.Session.Store(order);
		}
	}
}