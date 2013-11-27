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
using System.Text;
using CodeTitans.Bayeux;
using CodeTitans.Bayeux.Requests;
using CodeTitans.UnitTests.Bayeux.Model;
using System.Threading;
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
    public class BayeuxConnectionTests
    {
        [TestMethod]
        public void CreateBayeuxConnectionWithoutLongPolling()
        {
            var httpDataSource = new HttpDataSource("http://www.google.pl");
            var connection = new BayeuxConnection(httpDataSource, null);

            connection.LongPollingTimeout = 100;

            Assert.IsFalse(connection.IsLongPolling);
            Assert.AreEqual(0, connection.LongPollingTimeout);
        }

#if DEBUG
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TryToStartLongPollingRequestWithoutDefineLongPollingDataSource()
        {
            var httpDataSource = new HttpDataSource("http://www.google.pl");
            var connection = new BayeuxConnection(httpDataSource, null);

            connection.StartLongPolling(new HandshakeRequest(BayeuxConnectionTypes.LongPolling, null, null));
        }
#endif

        [TestMethod]
        public void CheckIfByDefaultLongPollingConnectionIsNotCreated()
        {
            var httpDataSource = new HttpDataSource("http://www.google.pl");
            var connection = new BayeuxConnection(httpDataSource); // <--- different constructor used, comparing to CreateBayeuxConnectionWithoutLongPolling() test

            connection.LongPollingTimeout = 100;

            Assert.IsFalse(connection.IsLongPolling);
            Assert.AreEqual(0, connection.LongPollingTimeout);
        }

        [TestMethod]
        public void InitializeLongPollingConnection()
        {
            var httpDataSource = new HttpDataSource("http://www.google.pl");
            var httpLongPollingDataSource = new HttpDataSource("http://www.google.pl/long");
            const int Timeout = 150;
            var connection = new BayeuxConnection(httpDataSource, httpLongPollingDataSource);

            connection.LongPollingTimeout = Timeout;

            Assert.AreEqual(Timeout, connection.LongPollingTimeout);
        }
    }
}
