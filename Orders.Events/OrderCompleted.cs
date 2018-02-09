using System;
using NServiceBus;

namespace Orders.Events
{
	public class OrderCompleted : IEvent
	{
		public Guid OrderId { get; set; }
	}
}