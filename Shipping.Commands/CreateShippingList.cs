using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Shipping.Commands
{
	public class CreateShippingList : ICommand
	{
		public Guid Id { get; set; }

		public Guid OrderId { get; set; }

		public DateTime DateCreated => DateTime.Now;
	}
}
