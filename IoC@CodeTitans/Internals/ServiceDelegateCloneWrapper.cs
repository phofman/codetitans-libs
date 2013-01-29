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
using System.Collections.Generic;

namespace CodeTitans.Services.Internals
{
    /// <summary>
    /// Wrapper for a service object instantiated by given delegate per each request.
    /// </summary>
    internal class ServiceDelegateCloneWrapper : ServiceObjectWrapper
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public ServiceDelegateCloneWrapper(IServiceProviderEx provider, object registeredServiceID, IEnumerable<Type> validators, ServiceCreationHandler serviceHandler, object[] serviceConstructorArgs)
            : base(provider)
        {
            if (serviceHandler == null)
                throw new ArgumentNullException("serviceHandler");

            RegisteredServiceID = registeredServiceID;
            Validators = validators;
            ServiceHandler = serviceHandler;
            ServiceArgs = serviceConstructorArgs;
        }

        #region Properties

        /// <summary>
        /// Gets the ID of the service passed during registration.
        /// </summary>
        protected object RegisteredServiceID
        { get; private set; }

        /// <summary>
        /// Gets the creation handler of the service object.
        /// </summary>
        protected ServiceCreationHandler ServiceHandler
        { get; private set; }

        /// <summary>
        /// Gets the constructor arguments for a service object.
        /// </summary>
        protected object[] ServiceArgs
        { get; private set; }

        /// <summary>
        /// Gets the collection of validators.
        /// </summary>
        protected IEnumerable<Type> Validators
        { get; private set; }

        #endregion

        /// <summary>
        /// Gets or creates an instance of a service object.
        /// </summary>
        public override object GetService(object requestedServiceName)
        {
            var e = new ServiceCreationEventArgs(Provider, RegisteredServiceID, requestedServiceName, ServiceArgs);

            // call the service creator:
            ServiceHandler(Provider, e);

            var service = e.Service;

            // check if service has been really created:
            if (service == null)
                return null;

            // validate if given service object
            // implements requested interfaces:
            if (Validators != null)
            {
                var serviceType = service.GetType();

                foreach (var v in Validators)
                    if (!v.IsAssignableFrom(serviceType))
                        throw new ServiceValidationException(v, serviceType);
            }

            IServiceSite site = service as IServiceSite;

            // initialize with proper service provider:
            if (site != null)
                site.SetSite(Provider);

            return service;
        }
    }
}
