using Common;
using NServiceBus;
using Raven.Client;
using Shipping.Commands;

namespace Shipping.Handlers
{
	public class CreateShippingListHandler : MessageHandlerBase<CreateShippingList>
	{
		protected override void HandleImpl(CreateShippingList message)
		{
			RavenSession.Session.Store(message);
		}
	}
}