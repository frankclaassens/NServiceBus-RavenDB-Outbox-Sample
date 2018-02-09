using System;
using Common;
using NServiceBus;
using NServiceBus.RavenDB.Persistence;
using Orders.Commands;
using Orders.Events;
using Raven.Client;
using Shipping.Commands;

namespace Shipping.Handlers
{
	public class OrderCreatedHandler : MessageHandlerBase<OrderCreated>
	{
		//private readonly ISessionProvider _session;
		//private readonly IDocumentSession _session;

		public OrderCreatedHandler(IBus bus)
		{
		}

		protected override void HandleImpl(OrderCreated message)
		{
			var order = RavenSession.Session.Load<CreateOrder>(message.OrderId);

			//var shippingList = new CreateShippingList
			//{
			//	OrderId = message.OrderId,
			//};

			//Bus.Send(shippingList);
		}
	}
}