using System;
using NServiceBus;

namespace Orders.Commands
{
	public class CreateOrder : ICommand
	{
		public Guid Id { get; set; }

		public string CustomerName { get; set; }

		public DateTime DateTime => DateTime.Now;
	}
}