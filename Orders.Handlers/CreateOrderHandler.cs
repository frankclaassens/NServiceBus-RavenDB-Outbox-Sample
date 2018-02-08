using System;
using NServiceBus;
using Orders.Commands;
using Orders.Events;
using NServiceBus.RavenDB.Persistence;

namespace Orders.Handlers
{
	public class CreateOrderHandler : IHandleMessages<CreateOrder>
	{
		private readonly IBus _bus;
		private readonly ISessionProvider _session;

		public CreateOrderHandler(ISessionProvider session, IBus bus)
		{
			_session = session;
			this._bus = bus;
		}

		public void Handle(CreateOrder message)
		{
			_session.Session.Store(message);

			_bus.Publish(new OrderCreated { OrderId = message.Id });
		}
	}
}