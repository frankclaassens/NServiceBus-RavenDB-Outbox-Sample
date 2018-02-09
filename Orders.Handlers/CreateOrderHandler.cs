using System;
using Common;
using NServiceBus;
using Orders.Commands;
using Orders.Events;
using NServiceBus.RavenDB.Persistence;
using Raven.Client;

namespace Orders.Handlers
{
	public class CreateOrderHandler : MessageHandlerBase<CreateOrder>
	{
		//private readonly ISessionProvider _session;
		//private readonly IDocumentSession _session;

		public CreateOrderHandler()
		{
		}

		protected override void HandleImpl(CreateOrder message)
		{
			RavenSession.Session.Store(message);

			//_bus.Publish(new OrderCreated { OrderId = message.Id });
		}
	}
}