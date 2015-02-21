#region License
/*
    Copyright (c) 2010, Paweł Hofman (CodeTitans)
    All Rights Reserved.

    Licensed under the Apache License version 2.0.
    For more information please visit:

    http://codetitans.codeplex.com/license
        or
    http://www.apache.org/licenses/


    For latest source code, documentation, samples
    and more information please visit:

    http://codetitans.codeplex.com/
*/
#endregion

using System;
using System.Reflection;

namespace CodeTitans.Services.Internals
{
    /// <summary>
    /// Wrapper for a service class that creates new object instance
    /// each time the service is requested.
    /// </summary>
    internal class ServiceTypeCloneWrapper : ServiceObjectWrapper
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public ServiceTypeCloneWrapper(IServiceProviderEx provider, Type serviceType, object[] serviceConstructorArgs)
            : base(provider)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            ServiceType = serviceType;
            ServiceArgs = serviceConstructorArgs != null && serviceConstructorArgs.Length > 0 ? serviceConstructorArgs : null;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the service object.
        /// </summary>
        protected Type ServiceType
        { get; private set; }

        /// <summary>
        /// Gets the constructor arguments for a service object.
        /// </summary>
        protected object[] ServiceArgs
        { get; private set; }

        #endregion

        /// <summary>
        /// Gets or creates an instance of a service object.
        /// </summary>
        public override object GetService(object requestedServiceName)
        {
            // create new instance of the service:
#if PocketPC
            var service = Activator.CreateInstance(ServiceType);
#else
            var service = CreateServiceInstance(Provider, ServiceType, ServiceArgs);
#endif

            if (service == null)
                throw new ServiceCreationException(ServiceType);

            IServiceSite site = service as IServiceSite;

            // update the reference to the service provider:
            if (site != null)
            {
#if PocketPC
                // call this function in place of constructor:
                site.SetSiteArguments(ServiceArgs);
#endif
                site.SetSite(Provider);
            }

            return service;
        }

#if !PocketPC
        private static object CreateServiceInstance(IServiceProviderEx provider, Type serviceType, object[] serviceArgs)
        {
#if WINDOWS_STORE || WINDOWS_APP

            // if arguments were specified directly, simply create new object:
            if (serviceArgs != null)
                return Activator.CreateInstance(serviceType, serviceArgs);

            // or loop till find first constructor, for which it was possible to create all required arguments:
            foreach (var method in serviceType.GetTypeInfo().DeclaredConstructors)
            {
                var paramTypes = method.GetParameters();
                var paramValues = new object[paramTypes.Length];
                bool failedToInitialize = false;

                for (int i = 0; i < paramTypes.Length && !failedToInitialize; i++)
                {
                    // get the service for specified argument's type (what returns null on failure):
                    paramValues[i] = provider.GetService(paramTypes[i].ParameterType);
                    failedToInitialize = paramValues[i] == null;
                }

                if (failedToInitialize)
                    continue;

                return method.Invoke(paramValues);
            }

            // notify, that it was impossible to initialize the required service
            throw new ServiceCreationException(serviceType);
#else

            // if arguments specified directly, simply create new object:
            if (serviceArgs != null)
                return Activator.CreateInstance(serviceType, serviceArgs);

            // or loop till find first constructor, for which it was possible to create all required arguments:
            foreach (var method in serviceType.GetConstructors())
            {
                var paramTypes = method.GetParameters();
                var paramValues = new object[paramTypes.Length];
                bool failedToInitialize = false;

                for (int i = 0; i < paramTypes.Length && !failedToInitialize; i++)
                {
                    // get the service for specified argument's type (what returns null on failure):
                    paramValues[i] = provider.GetService(paramTypes[i].ParameterType);
                    failedToInitialize = paramValues[i] == null;
                }

                if (failedToInitialize)
                    continue;

                return method.Invoke(paramValues);
            }

            // notify, that it was impossible to initialize the required service
            throw new ServiceCreationException(serviceType);

#endif
        }
#endif
    }
}
