using System;
using NServiceBus;
using NServiceBus.RavenDB.Persistence;
using Orders.Events;
using Raven.Client;
using Shipping.Commands;

namespace Shipping.Handlers
{
	public class CreateShippingListHandler : IHandleMessages<CreateShippingList>
	{
		private readonly IBus _bus;
		private readonly ISessionProvider _session;

		public CreateShippingListHandler(ISessionProvider session, IBus bus)
		{
			_session = session;
			this._bus = bus;
		}

		public void Handle(CreateShippingList message)
		{
			_session.Session.Store(message);			
		}
	}
}