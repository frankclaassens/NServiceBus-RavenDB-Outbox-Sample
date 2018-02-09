using System;
using Raven.Client;
using Raven.Json.Linq;

namespace Common
{
	public static class DocumentSessionExtensions
	{
		public static void StoreWithExpiration<TEntity>(this IDocumentSession ravenSession, TEntity entity, TimeSpan expireAfter)
		{
			DateTime expirationDate = DateTime.UtcNow.Add(expireAfter);
			ravenSession.Store(entity);
			ravenSession.Advanced.GetMetadataFor(entity)["Raven-Expiration-Date"] = new RavenJValue(expirationDate);
		}	
	}
}