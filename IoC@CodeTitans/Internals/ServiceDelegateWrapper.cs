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
    /// Wrapper for a singleton service object created by a delegate.
    /// </summary>
    internal class ServiceDelegateWrapper : ServiceDelegateCloneWrapper
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public ServiceDelegateWrapper(IServiceProviderEx provider, object registeredServiceID, IEnumerable<Type> validators, ServiceCreationHandler serviceHandler, object[] serviceConstructorArgs)
            : base(provider, registeredServiceID, validators, serviceHandler, serviceConstructorArgs)
        {
        }

        /// <summary>
        /// Gets or creates an instance of a service object.
        /// </summary>
        public override object GetService(object requestedServiceName)
        {
            // create service if needed:
            lock (this)
            {
                if (Service == null)
                    Service = base.GetService(requestedServiceName);
            }

            return Service;
        }
    }
}
