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

using CodeTitans.Services.Internals;

namespace CodeTitans.Services
{
    /// <summary>
    /// Context for registered services.
    /// </summary>
    public sealed class RegisteredServiceContext
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        internal RegisteredServiceContext(IServiceManager serviceManager, ServiceObjectWrapper serviceObject)
        {
            ServiceManager = serviceManager;
            ServiceObject = serviceObject;
        }

        /// <summary>
        /// Gets the reference to service manager that registered it.
        /// </summary>
        internal IServiceManager ServiceManager
        { get; private set; }

        /// <summary>
        /// Gets the reference to internal wrapper of the service object.
        /// </summary>
        internal ServiceObjectWrapper ServiceObject
        { get; private set; }
    }
}
