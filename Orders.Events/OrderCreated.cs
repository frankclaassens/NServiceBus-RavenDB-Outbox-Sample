using System;
using NServiceBus;

namespace Orders.Events
{
	public class OrderCreated : IEvent
	{
		public Guid OrderId { get; set; }
	}
}