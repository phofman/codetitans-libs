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
using CodeTitans.Services.Internals;
using System.Reflection;

namespace CodeTitans.Services
{
    /// <summary>
    /// Storage class for defining, creating and storing services.
    /// </summary>
    public sealed class ServiceLocator : IServiceProviderEx, IServiceManagerEx
    {
        private readonly ServiceCollection<Type> _typeServices = new ServiceCollection<Type>();
        private readonly ServiceCollection<string> _stringServices = new ServiceCollection<string>();
        private readonly ServiceCollection<Guid> _guidServices = new ServiceCollection<Guid>();
        private readonly ServiceCollection<uint> _uintServices = new ServiceCollection<uint>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServiceLocator()
        {
            RegisterDefaultServices();
        }

        /// <summary>
        /// Unregisters all services.
        /// </summary>
        public void Clear()
        {
            Clear(_typeServices);
            Clear(_stringServices);
            Clear(_guidServices);
            Clear(_uintServices);

            RegisterDefaultServices();
        }

        private void RegisterDefaultServices()
        {
            Register(typeof(IServiceProviderEx), this);
            Register(typeof(IServiceManagerByType), this);
            Register(typeof(IServiceManagerByString), this);
            Register(typeof(IServiceManagerByGuid), this);
            Register(typeof(IServiceManagerByUInt), this);
            Register(typeof(IServiceManagerEx), this);
        }

        #region Implementation of IServiceProvider

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return GetServiceObject(_typeServices, serviceType);
        }

        #endregion

        #region Implementation of IServiceProviderEx

        /// <summary>
        /// Gets the number of stored services.
        /// </summary>
        public int Count
        {
            get { return _typeServices.Count + _stringServices.Count + _guidServices.Count + _uintServices.Count; }
        }

        /// <summary>
        /// Gets service by a given type.
        /// </summary>
        public T GetService<T>() where T : class
        {
            return GetServiceObject(_typeServices, typeof (T)) as T;
        }

        /// <summary>
        /// Gets the service by a given type.
        /// </summary>
        public T GetService<TS, T>() where T : class where TS : class
        {
            return GetServiceObject(_typeServices, typeof(TS)) as T;
        }

        /// <summary>
        /// Gets the service by given name.
        /// </summary>
        public object GetService(string serviceName)
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            return GetServiceObject(_stringServices, serviceName);
        }

        /// <summary>
        /// Gets service by a given name.
        /// </summary>
        public T GetService<T>(string serviceName) where T : class
        {
            if (serviceName == null)
                throw new ArgumentNullException("serviceName");

            return GetServiceObject(_stringServices, serviceName) as T;
        }

        /// <summary>
        /// Gets the service by given Guid.
        /// </summary>
        public object GetService(Guid serviceId)
        {
            return GetServiceObject(_guidServices, serviceId);
        }

        /// <summary>
        /// Gets the service by given Guid.
        /// </summary>
        public T GetService<T>(Guid serviceId) where T : class
        {
            return GetServiceObject(_guidServices, serviceId) as T;
        }

        /// <summary>
        /// Gets the service by given id.
        /// </summary>
        public object GetService(uint serviceId)
        {
            return GetServiceObject(_uintServices, serviceId);
        }

        /// <summary>
        /// Gets the service by given id.
        /// </summary>
        public T GetService<T>(uint serviceId) where T : class
        {
            return GetServiceObject(_uintServices, serviceId) as T;
        }

        #endregion

        #region Implementation of IServiceManager

        /// <summary>
        /// Registers a given object as a singleton service by using all of its implemented interfaces.
        /// </summary>
        public void Register(object service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

#if WINDOWS_STORE
            Register(service.GetType().GetTypeInfo().ImplementedInterfaces, service);
#else
            Register(service.GetType().GetInterfaces(), service);
#endif
        }

        #endregion

        #region Implementation of IServiceManagerEx<Type>

        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        public RegisteredServiceContext Register(Type id, object service)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (service != null)
                VerifyServiceObjectType(id, service.GetType());

            return RegisterServiceObject(_typeServices, id, GetWrapper(service));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        public RegisteredServiceContext Register(Type id, Type serviceType, ServiceMode mode, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (serviceType != null)
                VerifyServiceObjectType(id, serviceType);

            return RegisterServiceObject(_typeServices, id, GetWrapper(serviceType, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        public RegisteredServiceContext Register(Type id, ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return RegisterServiceObject(_typeServices, id,
                                         GetWrapper(id, new[] {id}, creationHandler, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        public RegisteredServiceContext Bind(Type id, RegisteredServiceContext registeredServiceContext)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (registeredServiceContext == null)
                throw new ArgumentNullException("registeredServiceContext");
            if (registeredServiceContext.ServiceObject == null)
                throw new ArgumentException("Invalid context object", "registeredServiceContext");

            RegisterServiceObject(_typeServices, id, registeredServiceContext.ServiceObject);
            return registeredServiceContext;
        }

        #endregion

        #region Implementation of IServiceManagerEx<IEnumerable<Type>>

        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        public RegisteredServiceContext Register(IEnumerable<Type> id, object service)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (service != null)
                VerifyServiceObjectTypes(id, service.GetType());

            return RegisterServiceObject(_typeServices, id, GetWrapper(service));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        public RegisteredServiceContext Register(IEnumerable<Type> id, Type serviceType, ServiceMode mode, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            if (serviceType != null)
                VerifyServiceObjectTypes(id, serviceType);

            return RegisterServiceObject(_typeServices, id, GetWrapper(serviceType, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        public RegisteredServiceContext Register(IEnumerable<Type> id, ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return RegisterServiceObject(_typeServices, id, GetWrapper(id, id, creationHandler, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        public RegisteredServiceContext Bind(IEnumerable<Type> id, RegisteredServiceContext registeredServiceContext)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (registeredServiceContext == null)
                throw new ArgumentNullException("registeredServiceContext");
            if (registeredServiceContext.ServiceObject == null)
                throw new ArgumentException("Invalid context object", "registeredServiceContext");

            RegisterServiceObject(_typeServices, id, registeredServiceContext.ServiceObject);
            return registeredServiceContext;
        }

        #endregion

        #region Implementation of IServiceManagerEx<string>

        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        public RegisteredServiceContext Register(string id, object service)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return RegisterServiceObject(_stringServices, id, GetWrapper(service));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        public RegisteredServiceContext Register(string id, Type serviceType, ServiceMode mode, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return RegisterServiceObject(_stringServices, id, GetWrapper(serviceType, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        public RegisteredServiceContext Register(string id, ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (creationHandler == null)
                throw new ArgumentNullException("creationHandler");

            return RegisterServiceObject(_stringServices, id, GetWrapper(id, null, creationHandler, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        public RegisteredServiceContext Bind(string id, RegisteredServiceContext registeredServiceContext)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (registeredServiceContext == null)
                throw new ArgumentNullException("registeredServiceContext");
            if (registeredServiceContext.ServiceObject == null)
                throw new ArgumentException("Invalid context object", "registeredServiceContext");

            RegisterServiceObject(_stringServices, id, registeredServiceContext.ServiceObject);
            return registeredServiceContext;
        }

        #endregion

        #region Implementation of IServiceManagerEx<IEnumerable<string>>

        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        public RegisteredServiceContext Register(IEnumerable<string> id, object service)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            return RegisterServiceObject(_stringServices, id, GetWrapper(service));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        public RegisteredServiceContext Register(IEnumerable<string> id, Type serviceType, ServiceMode mode, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            return RegisterServiceObject(_stringServices, id, GetWrapper(serviceType, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        public RegisteredServiceContext Register(IEnumerable<string> id, ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (creationHandler == null)
                throw new ArgumentNullException("creationHandler");

            return RegisterServiceObject(_stringServices, id, GetWrapper(id, null, creationHandler, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        public RegisteredServiceContext Bind(IEnumerable<string> id, RegisteredServiceContext registeredServiceContext)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            if (registeredServiceContext == null)
                throw new ArgumentNullException("registeredServiceContext");
            if (registeredServiceContext.ServiceObject == null)
                throw new ArgumentException("Invalid context object", "registeredServiceContext");

            RegisterServiceObject(_stringServices, id, registeredServiceContext.ServiceObject);
            return registeredServiceContext;
        }

        #endregion

        #region Implementation of IServiceManagerEx<Guid>

        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        public RegisteredServiceContext Register(Guid id, object service)
        {
            return RegisterServiceObject(_guidServices, id, GetWrapper(service));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        public RegisteredServiceContext Register(Guid id, Type serviceType, ServiceMode mode, params object[] constructorArgs)
        {
            return RegisterServiceObject(_guidServices, id, GetWrapper(serviceType, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        public RegisteredServiceContext Register(Guid id, ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs)
        {
            return RegisterServiceObject(_guidServices, id, GetWrapper(id, null, creationHandler, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        public RegisteredServiceContext Bind(Guid id, RegisteredServiceContext registeredServiceContext)
        {
            if (registeredServiceContext == null)
                throw new ArgumentNullException("registeredServiceContext");
            if (registeredServiceContext.ServiceObject == null)
                throw new ArgumentException("Invalid context object", "registeredServiceContext");

            RegisterServiceObject(_guidServices, id, registeredServiceContext.ServiceObject);
            return registeredServiceContext;
        }

        #endregion

        #region Implementation of IServiceManagerEx<uint>

        /// <summary>
        /// Registers a given singleton service and binds it to given id.
        /// </summary>
        public RegisteredServiceContext Register(uint id, object service)
        {
            return RegisterServiceObject(_uintServices, id, GetWrapper(service));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime.
        /// </summary>
        public RegisteredServiceContext Register(uint id, Type serviceType, ServiceMode mode, params object[] constructorArgs)
        {
            return RegisterServiceObject(_uintServices, id, GetWrapper(serviceType, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds service with a given id to objects of specified type, created later at runtime by specified handler.
        /// </summary>
        public RegisteredServiceContext Register(uint id, ServiceMode mode, ServiceCreationHandler creationHandler, params object[] constructorArgs)
        {
            return RegisterServiceObject(_uintServices, id, GetWrapper(id, null, creationHandler, mode, constructorArgs));
        }

        /// <summary>
        /// Registers and binds a link to already registered service with another id.
        /// </summary>
        public RegisteredServiceContext Bind(uint id, RegisteredServiceContext registeredServiceContext)
        {
            if (registeredServiceContext == null)
                throw new ArgumentNullException("registeredServiceContext");
            if (registeredServiceContext.ServiceObject == null)
                throw new ArgumentException("Invalid context object", "registeredServiceContext");

            RegisterServiceObject(_uintServices, id, registeredServiceContext.ServiceObject);
            return registeredServiceContext;
        }

        #endregion

        #region Service Wrappers

        /// <summary>
        /// Gets a wrapper for an object.
        /// </summary>
        private ServiceObjectWrapper GetWrapper(object service)
        {
            return service == null ? null : new ServiceObjectWrapper(this, service);
        }

        /// <summary>
        /// Gets a wrapper dynamically creating service of specified type.
        /// </summary>
        private ServiceObjectWrapper GetWrapper(Type serviceType, ServiceMode mode, object[] constructorArgs)
        {
            if (serviceType == null)
                return null;

            return mode == ServiceMode.Singleton
                       ? new ServiceTypeWrapper(this, serviceType, constructorArgs)
                       : new ServiceTypeCloneWrapper(this, serviceType, constructorArgs);
        }

        /// <summary>
        /// Gets a wrapper dynamically creating/validating service via specified handler.
        /// </summary>
        private ServiceObjectWrapper GetWrapper(object id, IEnumerable<Type> validators, ServiceCreationHandler creationHandler, ServiceMode mode, object[] constructorArgs)
        {
            if (creationHandler == null)
                return null;

            return mode == ServiceMode.Singleton
                       ? new ServiceDelegateWrapper(this, id, validators, creationHandler, constructorArgs)
                       : new ServiceDelegateCloneWrapper(this, id, validators, creationHandler, constructorArgs);
        }

        #endregion

        #region Internals

        /// <summary>
        /// Gets the service object from a given dictionary.
        /// </summary>
        private static object GetServiceObject<T>(IDictionary<T, ServiceObjectWrapper> serviceDictionary, T key)
        {
            lock (serviceDictionary)
            {
                ServiceObjectWrapper serviceWrapper;

                // and return singleton/new instance of a service with given name:
                return serviceDictionary.TryGetValue(key, out serviceWrapper) ? serviceWrapper.GetService(key) : null;
            }
        }

        /// <summary>
        /// Verifies if given service extends a specified type.
        /// In case of relationship failure it throws an exceptions.
        /// </summary>
        private static void VerifyServiceObjectType(Type expectedType, Type serviceObjectType)
        {
            if (serviceObjectType != null && !IsAssignableFrom(expectedType, serviceObjectType))
                throw new ServiceValidationException(expectedType, serviceObjectType);
        }

        /// <summary>
        /// Verifies if given service extends a specified type.
        /// In case of relationship failure it throws an exceptions.
        /// </summary>
        private static void VerifyServiceObjectTypes(IEnumerable<Type> expectedTypes, Type serviceObjectType)
        {
            foreach (Type type in expectedTypes)
                VerifyServiceObjectType(type, serviceObjectType);
        }

        /// <summary>
        /// Registers a service into given dictionary for further access.
        /// </summary>
        private RegisteredServiceContext RegisterServiceObject<T>(IDictionary<T, ServiceObjectWrapper> serviceDictionary,
                                            T id, ServiceObjectWrapper serviceWrapper)
        {
            lock (serviceDictionary)
            {
                // in case the key is already in use:
                serviceDictionary.Remove(id);

                // and add (or overwrite if previous step removed):
                if (serviceWrapper != null)
                    serviceDictionary.Add(id, serviceWrapper);
            }

            // and return a wrapper to allow later binding to the same service object:
            return new RegisteredServiceContext(this, serviceWrapper);
        }

        /// <summary>
        /// Registers a service into given dictionary for further access.
        /// </summary>
        private RegisteredServiceContext RegisterServiceObject<T>(IDictionary<T, ServiceObjectWrapper> serviceDictionary,
            IEnumerable<T> ids, ServiceObjectWrapper serviceWrapper)
        {
            lock (serviceDictionary)
            {
                foreach (var id in ids)
                {
                    // in case the key is already in use:
                    serviceDictionary.Remove(id);

                    // and add (or overwrite if previous step removed):
                    if (serviceWrapper != null)
                        serviceDictionary.Add(id, serviceWrapper);
                }
            }

            // and return a wrapper to allow later binding to the same service object:
            return new RegisteredServiceContext(this, serviceWrapper);
        }

        /// <summary>
        /// Releases all the registered services.
        /// </summary>
        private static void Clear<T>(IDictionary<T, ServiceObjectWrapper> serviceDictionary)
        {
            lock(serviceDictionary)
            {
                serviceDictionary.Clear();
            }
        }

        #endregion

        #region Type-check Helpers

        internal static bool IsAssignableFrom(Type type, Type from)
        {
#if WINDOWS_STORE
            return type.GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());
#else
            return type.IsAssignableFrom(from);
#endif
        }

        #endregion
    }
}
