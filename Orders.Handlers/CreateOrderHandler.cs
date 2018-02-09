using System;
using Common;
using Orders.Commands;
using Shipping.Commands;

namespace Orders.Handlers
{
	public class CreateOrderHandler : MessageHandlerBase<CreateOrder>
	{

		protected override void HandleImpl(CreateOrder message)
		{
			RavenSession.Session.Store(message);

			var shippingList = new CreateShippingList
			{
				Id = Guid.NewGuid(),
				OrderId = message.Id
			};

			Bus.Send(shippingList);
		}
	}
}