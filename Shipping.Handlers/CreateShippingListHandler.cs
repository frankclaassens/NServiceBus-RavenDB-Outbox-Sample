using Common;
using NServiceBus;
using Raven.Client;
using Shipping.Commands;

namespace Shipping.Handlers
{
	public class CreateShippingListHandler : MessageHandlerBase<CreateShippingList>
	{
		//private readonly ISessionProvider _session;
		//private readonly IDocumentSession _session;

		public CreateShippingListHandler( IBus bus)
		{
		}

		protected override void HandleImpl(CreateShippingList message)
		{
			RavenSession.Session.Store(message);
		}
	}
}