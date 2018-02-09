using System;
using System.Linq;
using Common;
using log4net.Config;
using NServiceBus;
using NServiceBus.Persistence;
using NServiceBus.RavenDB.Persistence;
using NServiceBus.UnitOfWork;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.UniqueConstraints;
using StructureMap;
using StructureMap.Pipeline;

namespace Shipping.Endpoint
{
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
	{
		private const string EndpointName = "Shipping.Endpoint";

		private static readonly Type[] EventTypes = { typeof(NServiceBus.IEvent) };
		private static readonly Type[] CommandTypes = { typeof(NServiceBus.ICommand) };
		private static readonly Type[] MessageTypes = { typeof(NServiceBus.IMessage) };

		public EndpointConfig()
		{
			//LogManager.Use<Log4NetFactory>();
			XmlConfigurator.Configure();
		}

		public void Customize(BusConfiguration configuration)
		{
			// Bus configuration 
			configuration.EndpointName(EndpointName);
			configuration.EnableInstallers();

			configuration.UseTransport<MsmqTransport>();
			configuration.UseSerialization<XmlSerializer>();

			configuration.Transactions().DefaultTimeout(TimeSpan.FromMinutes(10));
			configuration.TimeToWaitBeforeTriggeringCriticalErrorOnTimeoutOutages(TimeSpan.FromSeconds(60));
			configuration.EnableOutbox();
			configuration.Transactions().DisableDistributedTransactions();

			//Bus persistence bootstrap
			var subscriptionDocumentStore = new DocumentStore
			{
				ConnectionStringName = "NServiceBus.Persistence",
				DefaultDatabase = EndpointName,
				EnlistInDistributedTransactions = false
			};
			subscriptionDocumentStore.Initialize();

			var documentStore = CreateDocumentStore();

			var persistance = configuration.UsePersistence<RavenDBPersistence>();
			persistance.SetDefaultDocumentStore(documentStore).DoNotSetupDatabasePermissions();
			persistance.UseDocumentStoreForSubscriptions(subscriptionDocumentStore);

			//Ioc bootstrap
			var container = new Container();

			BuildRegistry(container, documentStore);
			configuration.UseContainer<StructureMapBuilder>(c => c.ExistingContainer(container));
		}

		private void SetupConventions(BusConfiguration configuration)
		{
			configuration.Conventions().DefiningEventsAs(
				t => { return EventTypes.Any(et => et.IsAssignableFrom(t) && et != t); });
			configuration.Conventions().DefiningCommandsAs(
				t => { return t.IsClass && CommandTypes.Any(et => et.IsAssignableFrom(t) && et != t); });
			configuration.Conventions().DefiningMessagesAs(
				t => { return t.IsClass && MessageTypes.Any(et => et.IsAssignableFrom(t) && et != t); });
		}

		private void BuildRegistry(Container container, IDocumentStore documentStore)
		{
			container.Configure(x =>
			{
				//x.For<IManageUnitsOfWork>().Use<UnitOfWork>();

				x.ForSingletonOf<IDocumentStore>().LifecycleIs(new ContainerLifecycle()).Use(documentStore);
				//x.For<IDocumentSession>().Use(ctx => ctx.GetInstance<IDocumentStore>().OpenSession());
				x.For<IDocumentSession>().Use(ctx => ctx.GetInstance<ISessionProvider>().Session);

				x.Policies.SetAllProperties(t => t.OfType<IDocumentSession>());
				x.Policies.SetAllProperties(t => t.OfType<IBus>());
			});

		}

		private IDocumentStore CreateDocumentStore()
		{
			var store = CreateBaseDocumentStore(ConsistencyOptions.None, 15);

			store.Listeners.RegisterListener(new UniqueConstraintsStoreListener());

			store.EnlistInDistributedTransactions = false;

			//Enable optimisc concurrency on a global document store / session level.
			store.Conventions.DefaultUseOptimisticConcurrency = true;

			store.Initialize();

			return store;
		}

		public static DocumentStore CreateBaseDocumentStore(ConsistencyOptions consistencyType, int maxNumberOfRequests = 30)
		{
			return new DocumentStore
			{
				ConnectionStringName = "RavenDB",
				EnlistInDistributedTransactions = false,
				Conventions =
				{
					DefaultQueryingConsistency = consistencyType,
					MaxNumberOfRequestsPerSession = maxNumberOfRequests,
					DisableProfiling = true,
				},
			};
		}
	}
}