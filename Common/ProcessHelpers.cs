using System;
using System.Collections.Generic;
using System.Configuration;
using NServiceBus;
using NServiceBus.Config;
using Raven.Client;

namespace Common
{
	public static class ProcessHelpers
	{
		private static readonly int MaxFirstLevelRetries = GetMaxFirstLevelRetries();

		public static int GetMaxFirstLevelRetries()
		{
			// Semantically, MaxRetries is misleading - it actually means maximum number of attempts.
			var section = (TransportConfig)ConfigurationManager.GetSection("TransportConfig");
			return Math.Max(0, section.MaxRetries - 1);
		}

		public static void RegisterProcessEvent(Guid processId, IList<string> events, IDocumentSession ravenSession)
		{
			var processEvent = CreateProcessEvent(processId, events, null);
			ravenSession.StoreProcessEvent(processEvent);
			Console.WriteLine($"Process event registered - Id = {processId}, Events = '{string.Join(", ", events)}'.");
		}

		public static void RegisterErrorProcessEvent(Guid processId, string errorMessage, IBus bus, IDocumentSession ravenSession)
		{
			var processEvent = CreateProcessEvent(processId, null, bus, errorMessage);
			ravenSession.StoreProcessEvent(processEvent);
			Console.WriteLine($"Process event error registered = Id - {processId}, Error = '{errorMessage}', Message Id = {processEvent.Error.MessageId}.");
		}

		public static void RegisterProcessStatus(Guid processId, int messagesToProcess, IDocumentSession ravenSession)
		{
			var processEvent = CreateProcessStatus(processId, messagesToProcess, null);
			ravenSession.StoreProcessStatus(processEvent);
			Console.WriteLine($"Process registered - Id = {processId}, Delta = {messagesToProcess}.");
		}

		public static void RegisterErrorProcessStatus(Guid processId, string errorMessage, IBus bus, IDocumentSession ravenSession)
		{
			var processEvent = CreateProcessStatus(processId, 0, bus, errorMessage);
			ravenSession.StoreProcessStatus(processEvent);
			Console.WriteLine($"Process error registered - Id = {processId}, Error = '{errorMessage}', Message Id = {processEvent.Error.MessageId}.");
		}

		private static ProcessEvent CreateProcessEvent(Guid processId, IList<string> events, IBus bus, string errorMessage = null)
		{
			return new ProcessEvent { Id = $"{processId}/", Events = events, ProcessId = processId, Error = CreateProcessError(bus, errorMessage) };
		}

		private static void StoreProcessEvent(this IDocumentSession ravenSession, ProcessEvent processEvent)
		{
			ravenSession.StoreWithExpiration(processEvent, TimeSpan.FromMinutes(2));
		}

		private static ProcessStatus CreateProcessStatus(Guid processId, int processingDelta, IBus bus, string errorMessage = null)
		{
			return new ProcessStatus
			{
				Id = $"{processId}/",
				ProcessId = processId,
				ProcessingDelta = processingDelta,
				Error = CreateProcessError(bus, errorMessage)
			};
		}

		private static void StoreProcessStatus(this IDocumentSession ravenSession, ProcessStatus processStatus)
		{
			ravenSession.StoreWithExpiration(processStatus, TimeSpan.FromMinutes(5));
		}

		private static ProcessError CreateProcessError(IBus bus, string errorMessage)
		{
			if (errorMessage == null)
			{
				return null;
			}

			return new ProcessError { MaxAttempts = MaxFirstLevelRetries + 1, Message = errorMessage, MessageId = bus.GetMessageId() };
		}
	}

	public class ProcessEvent : ProcessBase
	{
		public IList<string> Events { get; set; }
	}
}
