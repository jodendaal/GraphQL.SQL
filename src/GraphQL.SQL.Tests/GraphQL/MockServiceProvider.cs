using GraphQL.DI;
using GraphQL.Types;
using System;
using System.Collections.Generic;

namespace GraphQL.SQL.Tests.Fields
{
    public sealed class MockServiceProvider : IServiceProvider
    {
        Dictionary<Type, object> types = new Dictionary<Type, object>();
        private readonly string connectionString;

        public void AddService(Type serviceType, object service)
        {
            types.Add(serviceType, service);
        }


        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            try
            {
                if (types.ContainsKey(serviceType))
                {
                    return types[serviceType];
                }
                if (serviceType == typeof(IEnumerable<IConfigureSchema>))
                {
                    return new List<ConfigurSchema>() { new ConfigurSchema() };
                }

                return Activator.CreateInstance(serviceType);
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to call Activator.CreateInstance. Type: {serviceType.FullName}", exception);
            }
        }

        public class ConfigurSchema : IConfigureSchema
        {
            public void Configure(ISchema schema, IServiceProvider serviceProvider)
            {

            }
        }
    }
}
