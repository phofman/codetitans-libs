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
            ServiceArgs = serviceConstructorArgs;
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
            var service = Activator.CreateInstance(ServiceType, ServiceArgs);
#endif

            if (service == null)
                throw new ServiceCreationException(ServiceType);

            IServiceSite site = service as IServiceSite;

            // update the reference to the service provider:
            if (site != null)
            {
#if PocketPC
                // call this function in place of contructor:
                site.SetSiteArguments(ServiceArgs);
#endif
                site.SetSite(Provider);
            }

            return service;
        }
    }
}
