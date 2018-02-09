using System;
using System.Collections.Generic;
using System.Linq;
using NServiceBus;

namespace Common
{
	public static class BusExtensions
	{
		private const string OutgoingTransportMessagesCountHeaderKey = "StudioHub.OutgoingTransportMessagesCount";
		private const string DoNotIncludeMessageInSynchronisation = "StudioHub.DoNotIncludeMessageInSynchronisation";
		private const string PublishedEventsHeaderKey = "StudioHub.PublishedEvents";
		private const string EnclosedMessageTypesHeaderKey = "NServiceBus.EnclosedMessageTypes";
		private const string MessageIdHeaderKey = "NServiceBus.MessageId";

		public const string UserIdHeaderKey = "UserId";
		public const string UserTypeHeaderKey = "UserType";

		public static Guid? GetExecutingUserId(this IBus bus)
		{
			if (bus.CurrentMessageContext == null)
				return null;

			string userId;
			if (!bus.CurrentMessageContext.Headers.TryGetValue(UserIdHeaderKey, out userId))
				return null;

			return Guid.Parse(userId);
		}

		public static Guid GetExecutingUserRef(this IBus bus)
		{
			Guid? userId = GetExecutingUserId(bus);

			if (userId == null)
				return Guid.Empty;

			return Guid.Empty;
		}

		public static Guid? GetProcessId(this IBus bus)
		{
			string processIdText;

			if (!bus.CurrentMessageContext.Headers.TryGetValue(ProcessStatus.ProcessIdHeaderKey, out processIdText))
			{
				return null;
			}

			return Guid.Parse(processIdText);
		}

		public static bool FullProcessMode(this IBus bus)
		{
			string processModeText;

			if (!bus.CurrentMessageContext.Headers.TryGetValue(ProcessStatus.FullProcessModeHeaderKey, out processModeText))
			{
				return false;
			}

			return bool.Parse(processModeText);
		}

		public static bool FirstMessageInProcess(this IBus bus)
		{
			string firstMessageText;

			if (!bus.CurrentMessageContext.Headers.TryGetValue(ProcessStatus.FirstMessageInProcessHeaderKey, out firstMessageText))
			{
				return false;
			}

			return bool.Parse(firstMessageText);
		}

		public static void IncrementOutgoingTransportMessagesCount(this IBus bus)
		{
			int count = bus.GetOutgoingTransportMessagesCount();
			count++;
			bus.CurrentMessageContext.Headers[OutgoingTransportMessagesCountHeaderKey] = count.ToString();
		}

		public static int GetOutgoingTransportMessagesCount(this IBus bus)
		{
			string countText;

			if (bus.CurrentMessageContext.Headers.TryGetValue(OutgoingTransportMessagesCountHeaderKey, out countText))
			{
				return int.Parse(countText);
			}

			return 0;
		}

		public static void SetPublishedEvents(this IBus bus, IEnumerable<string> publishedEvents)
		{
			List<string> currentlyPublishedEvents = bus.GetPublishedEvents().ToList();
			currentlyPublishedEvents.AddRange(publishedEvents);

			if (!currentlyPublishedEvents.Any())
			{
				return;
			}

			bus.CurrentMessageContext.Headers[PublishedEventsHeaderKey] = string.Join(",", currentlyPublishedEvents.Distinct());
		}

		public static string[] GetPublishedEvents(this IBus bus)
		{
			string eventsText;

			if (bus.CurrentMessageContext.Headers.TryGetValue(PublishedEventsHeaderKey, out eventsText))
			{
				return eventsText.Split(',');
			}

			return new string[0];
		}

		public static string EnclosedMessageTypes(this IBus bus)
		{
			if (bus == null)
			{
				return null;
			}

			string messageTypes;

			if (bus.CurrentMessageContext.Headers.TryGetValue(EnclosedMessageTypesHeaderKey, out messageTypes))
			{
				return messageTypes;
			}

			return null;
		}

		public static Guid GetMessageId(this IBus bus)
		{
			var messageIdText = bus.CurrentMessageContext.Headers[MessageIdHeaderKey];
			return Guid.Parse(messageIdText);
		}

		public static void DoNotIncludeInSynchronisation(this IBus bus, object message)
		{
			bus.SetMessageHeader(message, DoNotIncludeMessageInSynchronisation, bool.TrueString);
		}

		public static bool IncludeInSynchronisation(this IBus bus, TransportMessage message)
		{
			string value;

			if (message.Headers.TryGetValue(DoNotIncludeMessageInSynchronisation, out value))
			{
				return !bool.Parse(value);
			}

			return true;
		}
	}

	public class ProcessStatus : ProcessBase
	{
		public int ProcessingDelta { get; set; }
	}

	public abstract class ProcessBase
	{
		public const string ProcessIdHeaderKey = "StudioHub.ProcessId";

		public const string FullProcessModeHeaderKey = "StudioHub.FullProcessMode";

		public const string FirstMessageInProcessHeaderKey = "StudioHub.FirstMessageInProcess";

		public string Id { get; set; }

		public Guid ProcessId { get; set; }

		public ProcessError Error { get; set; }
	}

	public class ProcessError
	{
		public Guid MessageId { get; set; }

		public int MaxAttempts { get; set; }

		public string Message { get; set; }
	}
}