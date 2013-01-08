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
    /// Wrapper for a singleton service object with a delayed initialization.
    /// </summary>
    internal class ServiceTypeWrapper : ServiceTypeCloneWrapper
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public ServiceTypeWrapper(IServiceProviderEx provider, Type serviceType, object[] serviceConstructorArgs)
            : base(provider, serviceType, serviceConstructorArgs)
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
