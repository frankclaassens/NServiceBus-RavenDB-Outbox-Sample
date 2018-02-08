using System;
using NServiceBus;
using NServiceBus.RavenDB.Persistence;
using Orders.Commands;
using Orders.Events;
using Raven.Client;
using Shipping.Commands;

namespace Shipping.Handlers
{
	public class OrderCreatedHandler : IHandleMessages<OrderCreated>
	{
		private readonly IBus _bus;
		//private readonly ISessionProvider _session;
		private readonly IDocumentSession _session;

		public OrderCreatedHandler(IDocumentSession session, IBus bus)
		{
			_session = session;
			this._bus = bus;
		}

		public void Handle(OrderCreated message)
		{
			var shippingList = new CreateShippingList
			{
				OrderId = message.OrderId,
			};

			_bus.Send(shippingList);
		}
	}
}