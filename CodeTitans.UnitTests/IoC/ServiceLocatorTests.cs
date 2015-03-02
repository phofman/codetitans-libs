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
using CodeTitans.Services;
#if NUNIT
using NUnit.Framework;
using TestClassAttribute=NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute=NUnit.Framework.TestAttribute;
using TestInitializeAttribute=NUnit.Framework.SetUpAttribute;
using TestCleanupAttribute=NUnit.Framework.TearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CodeTitans.UnitTests.IoC
{
    /// <summary>
    /// This is a test class for ServiceLocatorTest and is intended
    /// to contain all ServiceLocatorTest Unit Tests
    ///</summary>
    [TestClass]
    public class ServiceLocatorTests
    {
        public const string TestGuidString = "F587A062-1E7C-4358-A0AB-09032A954C69";
        public readonly Guid TestGuid = new Guid(TestGuidString);
        public const uint TestId = 10101;

        public interface ITestService
        {
        }

        public interface IUnknownService
        {
        }

        public class SimpleService : ITestService
        {
        }

        public class SimpleSiteService : ITestService, IServiceSite, IUnknownService, ICloneable
        {
            public IServiceProviderEx Provider
            {
                get;
                private set;
            }

            #region IServiceSite Members

            public void SetSite(IServiceProviderEx provider)
            {
                Provider = provider;
            }

            public void SetSiteArguments(params object[] initArgs)
            {
            }

            #endregion

            #region ICloneable Members

            public object Clone()
            {
                return null;
            }

            #endregion
        }

        #region Test Services

        interface IServiceA1
        {
            int A1();
        }

        interface IServiceA2
        {
            int A2();
        }

        interface IServiceA3
        {
            int A3();
        }

        class ServA1 : IServiceA1
        {
            private readonly int _i;

            public ServA1(int i)
            {
                _i = i;
            }

            public int A1()
            {
                return _i;
            }
        }

        class ServA2 : IServiceA2
        {
            private readonly int _i;

            public ServA2(int i)
            {
                _i = i;
            }

            public int A2()
            {
                return _i;
            }
        }

        class ServA3 : IServiceA3
        {
            private readonly IServiceA1 _servA1;
            private readonly IServiceA2 _servA2;

            public ServA3(IServiceA1 service1, IServiceA2 service2)
            {
                _servA1 = service1;
                _servA2 = service2;
            }

            public int A3()
            {
                return _servA1.A1() + _servA2.A2();
            }
        }

        #endregion

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryRegisterNullTypeService()
        {
            var s = new ServiceLocator();

            // exception expected here:
            s.Register((Type) null, new object());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TryRegisterNullNameService()
        {
            var s = new ServiceLocator();

            // exception expected here:
            s.Register((string) null, new object());
        }

        [TestMethod]
        public void RegisterServiceViaObject()
        {
            var s = new ServiceLocator();
            var service = new SimpleService();

            s.Register(service);

            // since now on, the service should be available via number of ways
            // let's try most of them if we receive the same object:
            ITestService result = s.GetService<ITestService>();
            Assert.AreEqual(result, service);

            result = s.GetService(typeof(ITestService)) as ITestService;
            Assert.AreEqual(result, service);

            // registering service via object shouldn't automatically register classes,
            // but we always let the object to be instantiated, if not abstract:
            result = s.GetService<SimpleService>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterServiceViaUInt()
        {
            var s = new ServiceLocator();
            const uint ServiceID = 1u;
            const uint NonExistingServiceID = 2u;

            s.Register(ServiceID, ServiceMode.Singleton, (sender, e) => { e.Service = new SimpleService(); });

            SimpleService service = (SimpleService)s.GetService(ServiceID);
            SimpleService nonExistingService = (SimpleService) s.GetService(NonExistingServiceID);

            Assert.IsNotNull(service);
            Assert.IsNull(nonExistingService);
        }

        [TestMethod]
        public void RegisterServiceViaGuid()
        {
            var s = new ServiceLocator();
            var service = new SimpleService();

            s.Register(TestGuid, service);

            // since now on, the service should be available via number of ways
            // let's try most of them if we receive the same object:
            ITestService result = s.GetService<ITestService>(TestGuid);
            Assert.AreEqual(result, service);

            Assert.AreEqual(s.GetService(TestGuid), service);

            result = s.GetService(typeof(ITestService)) as ITestService;
            Assert.IsNull(result);

            // registering service via object shouldn't automatically register classes,
            // but we always let the object to be instantiated, if not abstract:
            result = s.GetService<SimpleService>();
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterServiceViaGuidAndType()
        {
            var s = new ServiceLocator();

            s.Register(TestGuid, typeof(SimpleService), ServiceMode.Clone);

            Assert.IsNotNull(s.GetService(TestGuid));
        }

        [TestMethod]
        public void RegisterAndUnregisterService()
        {
            var s = new ServiceLocator();
            var service = new SimpleService();

            s.Register(TestGuid, service);
            s.Register(TestGuidString, service);
            s.Register(service);
            s.Register(TestId, service);

            // check if was added:
            Assert.AreEqual(service, s.GetService<ITestService>());
            Assert.AreEqual(service, s.GetService(TestGuid));
            Assert.AreEqual(service, s.GetService(TestGuidString));
            Assert.AreEqual(service, s.GetService(TestId));

            // passing the null value instead of real service, will release
            // service object:
            s.Register(typeof(ITestService), null);
            s.Register(TestGuid, null);
            s.Register(TestGuidString, null);
            s.Register(TestId, null);

            // validate if service is no more available:
            Assert.IsNull(s.GetService<ITestService>());
            Assert.IsNull(s.GetService(TestGuid));
            Assert.IsNull(s.GetService(TestGuidString));
            Assert.IsNull(s.GetService(TestId));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceValidationException))]
        public void FailToRegisterDynamicNoService()
        {
            var s = new ServiceLocator();

            // try to register service that doesn't implement required interface:
            s.Register(typeof(IUnknownService), typeof(ITestService));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceValidationException))]
        public void FailToRegisterDynamicService()
        {
            var s = new ServiceLocator();

            // try to register service that doesn't implement required interface:
            s.Register(typeof(IUnknownService), typeof(SimpleService));
        }

        [TestMethod]
        [ExpectedException(typeof(ServiceValidationException))]
        public void FailToRegisterDynamicServiceViaHandler()
        {
            var s = new ServiceLocator();
            ServiceCreationHandler h = (sender, e) =>
                                           {
                                               e.Service = new SimpleService();
                                           };

            // fail to register service:
            s.Register(typeof(IUnknownService), ServiceMode.Singleton, h);

            // try to get a service, and an exception should be generated:
            var o = s.GetService<IUnknownService>();

            Assert.IsNull(o);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FailToRegisterServiceWithNullStringID()
        {
            var s = new ServiceLocator();

            s.Register((string)null, typeof(SimpleService), ServiceMode.Clone);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FailToRegisterServiceWithNullType()
        {
            var s = new ServiceLocator();

            s.Register("Service1", null, ServiceMode.Clone);
        }

        [TestMethod]
        public void RegisterDynamicCloneService()
        {
            var s = new ServiceLocator();

            // register one that per each request will generate new instance of service:
            s.Register(typeof(ITestService), typeof(SimpleService), ServiceMode.Clone);

            // get reference to services:
            var service1 = s.GetService<ITestService>();
            var service2 = s.GetService<ITestService>();
            var service3 = s.GetService<ITestService>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service3);
            Assert.AreNotEqual(service1, service2);
            Assert.AreNotEqual(service1, service3);
        }

        [TestMethod]
        public void RegisterDynamicSingletonService()
        {
            var s = new ServiceLocator();

            // register service returning the same instance each time:
            s.Register(typeof(ITestService), typeof(SimpleService), ServiceMode.Singleton);

            // get reference to services:
            var service1 = s.GetService<ITestService>();
            var service2 = s.GetService<ITestService>();
            var service3 = s.GetService<ITestService>();

            Assert.IsNotNull(service1);
            Assert.IsNotNull(service2);
            Assert.IsNotNull(service3);
            Assert.AreEqual(service1, service2);
            Assert.AreEqual(service1, service3);
        }

        [TestMethod]
        public void RegisterSiteService()
        {
            var s = new ServiceLocator();
            var service = new SimpleSiteService();

            // all kinds of services should support IServiceSite call:
            s.Register(typeof(ITestService), typeof(SimpleSiteService), ServiceMode.Clone);
            s.Register(typeof(ICloneable), ServiceMode.Clone, (sender, e) => { e.Service = new SimpleSiteService(); });
            s.Register(typeof(IUnknownService), service);
            s.Register(TestId, typeof(SimpleSiteService), ServiceMode.Singleton);

            // get not null services with Provider set:
            var s1 = s.GetService<ITestService, SimpleSiteService>();
            var s2 = s.GetService<ICloneable, SimpleSiteService>();
            var s3 = s.GetService<IUnknownService, SimpleSiteService>();
            var s4 = s.GetService<SimpleSiteService>(TestId);

            Assert.IsNotNull(s1);
            Assert.IsNotNull(s1.Provider);
            Assert.AreEqual(s1.Provider, s);

            Assert.IsNotNull(s2);
            Assert.IsNotNull(s2.Provider);
            Assert.AreEqual(s2.Provider, s);

            Assert.IsNotNull(s3);
            Assert.IsNotNull(s3.Provider);
            Assert.AreEqual(s3.Provider, s);
            Assert.AreEqual(s3, service);

            Assert.IsNotNull(s4);
            Assert.IsNotNull(s4.Provider);
            Assert.AreEqual(s4.Provider, s);
        }

        [TestMethod]
        public void CheckIfClearPreservesDefaultSetOfServices()
        {
            var locator = new ServiceLocator();
            var defaultNumberOfServices = locator.Count;

            Assert.AreNotEqual(0, defaultNumberOfServices);

            locator.Register(1u, new SimpleService());
            Assert.AreNotEqual(defaultNumberOfServices, locator.Count);

            locator.Clear();
            Assert.AreEqual(defaultNumberOfServices, locator.Count);
        }

        [TestMethod]
        public void RemovesAllRegisteredServices()
        {
            var locator = new ServiceLocator();
            var service = new SimpleService();
            var defaultNumberOfSerices = locator.Count;

            locator.Register(Guid.NewGuid(), service);
            locator.Register(new[] { typeof(ITestService), typeof(SimpleService) }, service);
            locator.Register(new[] { "Service1", "Service2" }, service);
            locator.Register(Guid.NewGuid(), ServiceMode.Clone, (sender, e) => { e.Service = new SimpleService(); });
            locator.Register(1u, service);
            locator.Register("Serice3", typeof(SimpleService), ServiceMode.Clone);

            Assert.AreNotEqual(0, locator.Count);
            Assert.AreEqual(8 + defaultNumberOfSerices, locator.Count);

            locator.Clear();
            Assert.AreEqual(defaultNumberOfSerices, locator.Count);

            var newService = locator.GetService(typeof(SimpleService));
            Assert.IsNotNull(newService);

            newService = locator.GetService(1u);
            Assert.IsNull(newService);

            newService = locator.GetService("Service1");
            Assert.IsNull(newService);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void BindToNull()
        {
            var s = new ServiceLocator();

            // create any servie:
            var r = s.Register(typeof(ITestService), typeof(SimpleSiteService), ServiceMode.Clone);

            // try to bind with null:
            s.Bind(typeof(SimpleSiteService), null);

            Assert.IsNull(r);
        }

        [TestMethod]
        public void BindToExistingTypeService()
        {
            var s = new ServiceLocator();
            const string name1 = "MyService1";
            const string name2 = "MyService2";

            // create any servie:
            var r = s.Register(typeof(ITestService), typeof(SimpleSiteService), ServiceMode.Singleton);

            // try to bind with null:
            s.Bind(typeof(SimpleSiteService), r);
            s.Bind(name1, r);
            s.Bind(name2, r);

            // get services:
            var o1 = s.GetService<ITestService>();
            var o2 = s.GetService<ITestService>(name1);
            var o3 = s.GetService<ITestService>(name2);

            Assert.IsNotNull(r);
            Assert.IsNotNull(o1);
            Assert.IsNotNull(o2);
            Assert.IsNotNull(o3);

            Assert.AreEqual(o1, o2);
            Assert.AreEqual(o2, o3);
        }

        [TestMethod]
        public void CrossLocatorBinding()
        {
            var s1 = new ServiceLocator();
            var s2 = new ServiceLocator();

            // register any service:
            var r1 = s1.Register(typeof(ITestService), typeof(SimpleSiteService), ServiceMode.Singleton);
            Assert.IsNotNull(r1);

            // try to bind in other locator:
            var r2 = s2.Bind(typeof(ITestService), r1);

            // since it's possible to share the same service across multiple service locators,
            // expect the object to be identical:
            Assert.IsNotNull(r2);
            Assert.AreEqual(r1, r2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FailInCreationServiceCreationExeption()
        {
            new ServiceCreationException(null);
        }

        [TestMethod]
        public void CreateServiceCreationException()
        {
            var ex = new ServiceCreationException(typeof(int));

            Assert.IsNotNull(ex, "Shouldn't be null!");
        }

        [TestMethod]
        public void CreteServicesWithConstructorDependencies()
        {
            var s = new ServiceLocator();

            s.Register<IServiceA1, ServA1>(ServiceMode.Singleton, 33);
            s.Register<IServiceA2, ServA2>(ServiceMode.Singleton, 1);
            s.Register<IServiceA3, ServA3>(ServiceMode.Singleton);

            var item = s.GetService<IServiceA3>();
            Assert.IsNotNull(item, "Unable to create complex object");
            Assert.AreEqual(34, item.A3());

            var item2 = s.GetService<IServiceA3>();
            Assert.IsNotNull(item2, "Unable to get singleton instance of created object");
            Assert.IsTrue(ReferenceEquals(item, item2), "Invalid singleton instance");
        }

        [TestMethod]
        public void CreteServiceDirectlyWithConstructorDependencies()
        {
            var s = new ServiceLocator();

            s.Register<IServiceA1, ServA1>(ServiceMode.Singleton, 33);
            s.Register<IServiceA2, ServA2>(ServiceMode.Singleton, 1);
            s.Register<IServiceA3, ServA3>(ServiceMode.Singleton);

            var item = s.GetService<ServA3>();
            Assert.IsNotNull(item, "Unable to create complex object");
            Assert.AreEqual(34, item.A3());

            var item2 = s.GetService<IServiceA3>();
            Assert.IsNotNull(item2, "Unable to get another instance of created object");
            Assert.IsFalse(ReferenceEquals(item, item2), "Invalid instance");

            var item3 = s.GetService<IServiceA3, ServA3>();
            Assert.IsNotNull(item3, "Unable to create complex object");

            var item4 = s.GetService(typeof(ServA3));
            Assert.IsNotNull(item4, "Unable to create complex object");

        }
    }
}