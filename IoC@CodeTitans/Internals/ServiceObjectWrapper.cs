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
    /// Wrapper object for service instantiation.
    /// </summary>
    internal class ServiceObjectWrapper
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public ServiceObjectWrapper(IServiceProviderEx provider, object service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            Provider = provider;
            Service = service;

            IServiceSite site = service as IServiceSite;

            // update the provider also for a given service:
            if (site != null)
                site.SetSite(provider);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        protected ServiceObjectWrapper (IServiceProviderEx provider)
        {
            Provider = provider;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the service provider.
        /// </summary>
        protected IServiceProviderEx Provider
        { get; private set; }

        /// <summary>
        /// Gets an instance of a service object.
        /// </summary>
        protected object Service
        { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets or creates an instance of a service object.
        /// </summary>
        public virtual object GetService (object requestedServiceName)
        {
            return Service;
        }

        #endregion
    }
}
