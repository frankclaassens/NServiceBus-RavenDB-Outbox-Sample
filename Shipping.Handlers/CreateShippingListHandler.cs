using NServiceBus;
using Raven.Client;
using Shipping.Commands;

namespace Shipping.Handlers
{
	public class CreateShippingListHandler : IHandleMessages<CreateShippingList>
	{
		//private readonly ISessionProvider _session;
		private readonly IDocumentSession _session;

		public CreateShippingListHandler(IDocumentSession session, IBus bus)
		{
			_session = session;
		}

		public void Handle(CreateShippingList message)
		{
			_session.Store(message);			
		}
	}
}