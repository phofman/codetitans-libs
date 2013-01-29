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
    /// Interface for accessing any types of services.
    /// </summary>
    public interface IServiceProviderEx : IServiceProvider
    {
        /// <summary>
        /// Gets the number of stored services.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Gets service by a given type.
        /// </summary>
        T GetService<T>() where T : class;

        /// <summary>
        /// Gets the service by a given type.
        /// </summary>
        T GetService<TS, T>()
            where T : class
            where TS : class;

        /// <summary>
        /// Gets the service by given name.
        /// </summary>
        object GetService(string serviceName);

        /// <summary>
        /// Gets service by a given name.
        /// </summary>
        T GetService<T>(string serviceName) where T : class;

        /// <summary>
        /// Gets the service by given Guid.
        /// </summary>
        object GetService(Guid serviceId);

        /// <summary>
        /// Gets the service by given Guid.
        /// </summary>
        T GetService<T>(Guid serviceId) where T : class;

        /// <summary>
        /// Gets the service by given id.
        /// </summary>
        object GetService(uint serviceId);

        /// <summary>
        /// Gets the service by given id.
        /// </summary>
        T GetService<T>(uint serviceId) where T : class;
    }
}