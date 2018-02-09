using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using NServiceBus;
using NServiceBus.RavenDB.Persistence;
using Raven.Client;

namespace Common
{
	public abstract class MessageHandlerBase<TMessage> : IHandleMessages<TMessage>
	{
		public IBus Bus { get; set; }

		public ISessionProvider RavenSession { get; set; }

		public void Handle(TMessage message)
		{
			try
			{
				HandleImpl(message);
			}
			catch (Exception exception)
			{
				string errorMessage =
					$"Error occurred within handler {GetType().Name}";
				throw new HandlerException(errorMessage, exception);
			}
		}

		protected abstract void HandleImpl(TMessage message);
	}

	[Serializable]
	public class HandlerException : Exception
	{
		public HandlerException()
		{
		}

		public HandlerException(string message)
			: base(message)
		{
		}

		public HandlerException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected HandlerException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}