using NServiceBus;
using NServiceBus.RavenDB.Persistence;
using Raven.Client;

namespace Common
{
	internal class TrackingPipelineStepsRegistration : INeedInitialization
	{
		public void Customize(BusConfiguration busConfiguration)
		{
			//busConfiguration.RegisterComponents(e => e.ConfigureComponent<>(DependencyLifecycle.InstancePerCall));
			//busConfiguration.RegisterComponents(e => e.ConfigureComponent<UnitOfWork>(DependencyLifecycle.InstancePerUnitOfWork));			
		}
	}
}