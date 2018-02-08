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
		private readonly ISessionProvider _session;

		public OrderCreatedHandler(ISessionProvider session, IBus bus)
		{
			_session = session;
			this._bus = bus;
		}

		public void Handle(OrderCreated message)
		{
			var order = _session.Session.Load<CreateOrder>(message.OrderId);

			var shippingList = new CreateShippingList
			{
				OrderId = message.OrderId,
			};

			_bus.Send(shippingList);
		}
	}
}