using System;
using NServiceBus;
using Orders.Commands;
using Orders.Events;
using NServiceBus.RavenDB.Persistence;
using Raven.Client;

namespace Orders.Handlers
{
	public class CreateOrderHandler : IHandleMessages<CreateOrder>
	{
		private readonly IBus _bus;
		//private readonly ISessionProvider _session;
		private readonly IDocumentSession _session;

		public CreateOrderHandler(IDocumentSession session, IBus bus)
		{
			_session = session;
			this._bus = bus;
		}

		public void Handle(CreateOrder message)
		{
			_session.Store(message);

			_bus.Publish(new OrderCreated { OrderId = message.Id });
		}
	}
}