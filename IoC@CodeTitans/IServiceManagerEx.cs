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

namespace CodeTitans.Services
{
    /// <summary>
    /// Interface for defining services in more extensible way.
    /// </summary>
    public interface IServiceManagerEx<T> : IServiceManager
    {
        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        RegisteredServiceContext Register(T id, object service);

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        RegisteredServiceContext Register(T id, Type serviceType, ServiceMode mode,
                             params object[] constructorArgs);

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        RegisteredServiceContext Register(T id, ServiceMode mode, ServiceCreationHandler creationHandler,
                             params object[] constructorArgs);

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        RegisteredServiceContext Bind(T id, RegisteredServiceContext registeredServiceContext);
    }

    /// <summary>
    /// Interface defining services accessible by type.
    /// </summary>
    public interface IServiceManagerByType : IServiceManagerEx<Type>, IServiceManagerEx<IEnumerable<Type>>
    {
        /// <summary>
        /// Registers a given singleton service and binds it to given type.
        /// </summary>
        RegisteredServiceContext Register<T>(object service);

        /// <summary>
        /// Registers and binds service with a given type to objects of specified type, created later at runtime.
        /// </summary>
        RegisteredServiceContext Register<T>(Type serviceType, ServiceMode mode, params object[] constructorArgs);

        /// <summary>
        /// Registers and binds service with a given type to objects of specified type, created later at runtime.
        /// </summary>
        RegisteredServiceContext Register<T, TS>(ServiceMode mode, params object[] constructorArgs);

        /// <summary>
        /// Registers and binds service with a given type to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        RegisteredServiceContext Register<T>(ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs);

        /// <summary>
        /// Registers and binds a link to already registered service with another type.
        /// </summary>
        RegisteredServiceContext Bind<T>(RegisteredServiceContext registeredServiceContext);
    }

    /// <summary>
    /// Interface defining services accessible by string names.
    /// </summary>
    public interface IServiceManagerByString : IServiceManagerEx<string>, IServiceManagerEx<IEnumerable<string>>
    {
    }

    /// <summary>
    /// Interface defining services accessible by Guid names.
    /// </summary>
    public interface IServiceManagerByGuid : IServiceManagerEx<Guid>
    {
    }

    /// <summary>
    /// Interface defining services accessible by uint ids.
    /// </summary>
    public interface IServiceManagerByUInt : IServiceManagerEx<uint>
    {
    }

    /// <summary>
    /// Interface providing registration access to any kind of services.
    /// </summary>
    public interface IServiceManagerEx : IServiceManagerByType, IServiceManagerByString, IServiceManagerByUInt, IServiceManagerByGuid
    {
    }
}
