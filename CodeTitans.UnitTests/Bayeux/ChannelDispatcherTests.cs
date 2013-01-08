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
using CodeTitans.Bayeux.Channels;
#if NUNIT
using NUnit.Framework;
using TestClassAttribute=NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute=NUnit.Framework.TestAttribute;
using TestInitializeAttribute=NUnit.Framework.SetUpAttribute;
using TestCleanupAttribute=NUnit.Framework.TearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CodeTitans.UnitTests.Bayeux
{
    [TestClass]
    public class ChannelDispatcherTests
    {
        class CounterHandler : IChannelHandler
        {
            public int Calls
            {
                get;
                private set;
            }

            #region Implementation of IChannelHandler

            public void Handle(string channel, string[] segments, object data, object state)
            {
                Calls++;
            }

            #endregion
        }

        [TestMethod]
        public void CreateNewChannelDispatcher()
        {
            var dispatcher = new ChannelDispatcher();

            Assert.IsNotNull(dispatcher);
        }

        [TestMethod]
        public void CreateNewChannelDispatcherWithDefaultHandler()
        {
            var dispatcher = new ChannelDispatcher();
            var defaultDispatcher = new CounterHandler();

            dispatcher.RegisterDefault(defaultDispatcher);
            Assert.IsNotNull(dispatcher.DefaultHandler);

            dispatcher.Handle("/services/test", "data...");
            Assert.AreEqual(1, defaultDispatcher.Calls, "Invalid number of notifications!");
        }

        [TestMethod]
        public void TestBasicChannelMatchingWith2Handlers()
        {
            var dispatcher = new ChannelDispatcher();
            var handler1 = new CounterHandler();
            var handler2 = new CounterHandler();
            var defaultHandler = new CounterHandler();

            dispatcher.Register("/services/test/1", handler1);
            dispatcher.Register("/services/test/2", handler2);
            dispatcher.RegisterDefault(defaultHandler);
            Assert.IsNotNull(dispatcher.DefaultHandler);

            dispatcher.Handle("/services/test/2", "data...");
            Assert.AreEqual(1, handler2.Calls, "Invalid number of test/2 notifications!");
            Assert.AreEqual(0, handler1.Calls, "Too many test/2 handler calls on handler1!");
            Assert.AreEqual(0, defaultHandler.Calls, "Too many test/2 handler calls on defaultHandler!");

            dispatcher.Handle("/services/test/1", "data...");
            Assert.AreEqual(1, handler2.Calls, "Invalid number of test/2 notifications!");
            Assert.AreEqual(1, handler1.Calls, "Invalid number of test/1 notifications!");
            Assert.AreEqual(0, defaultHandler.Calls, "Too many test/1 handler calls on defaultHandler!");
        }

        [TestMethod]
        public void TestWildcardBasicChannelMatchingWith3Handlers()
        {
            var dispatcher = new ChannelDispatcher();
            var handler1 = new CounterHandler();
            var handler2 = new CounterHandler();
            var handler3 = new CounterHandler();
            var defaultHandler = new CounterHandler();

            dispatcher.Register("/services/test/*/edit", handler1);
            dispatcher.Register("/services/test/2", handler2);
            dispatcher.Register("/services/banana/*", handler2);
            dispatcher.Register("/services/internal", handler3);
            dispatcher.RegisterDefault(defaultHandler);
            Assert.IsNotNull(dispatcher.DefaultHandler);

            dispatcher.Handle("/services/test/1", "data..."); // hits default
            Assert.AreEqual(1, defaultHandler.Calls);
            Assert.AreEqual(0, handler1.Calls);
            Assert.AreEqual(0, handler2.Calls);
            Assert.AreEqual(0, handler3.Calls);

            dispatcher.Handle("/services/internal", "data..."); // hits handler3
            Assert.AreEqual(1, defaultHandler.Calls);
            Assert.AreEqual(0, handler1.Calls);
            Assert.AreEqual(0, handler2.Calls);
            Assert.AreEqual(1, handler3.Calls);

            dispatcher.Handle("/services/test/1/edit", "data..."); // hits handler1
            Assert.AreEqual(1, defaultHandler.Calls);
            Assert.AreEqual(1, handler1.Calls);
            Assert.AreEqual(0, handler2.Calls);
            Assert.AreEqual(1, handler3.Calls);

            dispatcher.Handle("/services/banana/edit", "data..."); // hits handler2
            Assert.AreEqual(1, defaultHandler.Calls);
            Assert.AreEqual(1, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(1, handler3.Calls);

            dispatcher.Handle("/services/banana/1/edit", "data..."); // hits default
            Assert.AreEqual(2, defaultHandler.Calls);
            Assert.AreEqual(1, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(1, handler3.Calls);
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void CreateInvalidGlobalWildcardHandlerChannel()
        {
            var dispatcher = new ChannelDispatcher();
            var handler1 = new CounterHandler();

            dispatcher.Register("/services/test/**/edit", handler1);
        }

        [TestMethod]
        public void TestGlobalWildcardChannelMatching()
        {
            var dispatcher = new ChannelDispatcher();
            var handler1 = new CounterHandler();
            var handler2 = new CounterHandler();
            var handler3 = new CounterHandler();
            var defaultHandler = new CounterHandler();

            dispatcher.Register("/services/test/**", handler1);
            dispatcher.Register("/services/test", handler2);
            dispatcher.Register("/services/banana/*", handler2);
            dispatcher.Register("/services/banana/**", handler3);
            dispatcher.RegisterDefault(defaultHandler);
            Assert.IsNotNull(dispatcher.DefaultHandler);

            dispatcher.Handle("/services/test", "data..."); // hits handler2
            Assert.AreEqual(0, defaultHandler.Calls);
            Assert.AreEqual(0, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(0, handler3.Calls);

            dispatcher.Handle("/services/test/edit", "data..."); // hits handler1
            Assert.AreEqual(0, defaultHandler.Calls);
            Assert.AreEqual(1, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(0, handler3.Calls);

            dispatcher.Handle("/services/test/1/edit", "data..."); // hits handler1
            Assert.AreEqual(0, defaultHandler.Calls);
            Assert.AreEqual(2, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(0, handler3.Calls);

            dispatcher.Handle("/services/banana", "data..."); // hits default handler
            Assert.AreEqual(1, defaultHandler.Calls);
            Assert.AreEqual(2, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(0, handler3.Calls);

            dispatcher.Handle("/services/banana/1/edit", "data..."); // hits handler3
            Assert.AreEqual(1, defaultHandler.Calls);
            Assert.AreEqual(2, handler1.Calls);
            Assert.AreEqual(1, handler2.Calls);
            Assert.AreEqual(1, handler3.Calls);
        }
    }
}
