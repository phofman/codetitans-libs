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

namespace CodeTitans.Services
{
    /// <summary>
    /// Arguments passed during the creation of the service object.
    /// </summary>
    public sealed class ServiceCreationEventArgs : EventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public ServiceCreationEventArgs(IServiceProviderEx provider, object registeredServiceID, object requestedServiceID, object[] serviceArgs)
        {
            Provider = provider;
            RegisteredServiceID = registeredServiceID;
            RequestedServiceID = requestedServiceID;
            ServiceArgs = serviceArgs;
        }

        #region Properties

        /// <summary>
        /// Gets the service provider.
        /// </summary>
        public IServiceProviderEx Provider
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the arguments passed to the creation function.
        /// </summary>
        public object[] ServiceArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID of the service passed during registration request.
        /// </summary>
        public object RegisteredServiceID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID of the service requested during GetService() call.
        /// </summary>
        public object RequestedServiceID
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the service object.
        /// </summary>
        public object Service
        {
            get;
            set;
        }

        #endregion
    }
}