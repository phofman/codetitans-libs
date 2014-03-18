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

using CodeTitans.Bayeux;
using CodeTitans.Diagnostics;
using CodeTitans.UnitTests.Bayeux.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeTitans.UnitTests.Bayeux
{
    [TestClass]
    public class HttpTests
    {
#if DEBUG
        [TestMethod]
        public void LoadHttpData()
        {
            var request = new HttpDataSource("https://www.google.com?q=a");
            var waiter = new AsyncWaiter();
            var filter = new FilterDebugListener("Filter HttpDataSource", "Core.HttpDataSource");

            DebugLog.AddListener(filter);

            request.SendRequestAsync(HttpDataSource.MethodGet, HttpDataSourceResponseType.AsString);
            Assert.AreEqual(WaiterResults.Success, waiter.Wait(10, request), "Couldn't load data");

            Assert.AreNotEqual(0, filter.Count, "Invalid number of captured log messages!");
        }
#endif

        [TestMethod]
        public void LoadHttpDataFollowedByCancel()
        {
            var request = new HttpDataSource("https://www.google.com?q=a");
            var waiter = new AsyncWaiter();

            request.SendRequestAsync(HttpDataSource.MethodGet, HttpDataSourceResponseType.AsString);
            request.Cancel();
            request.SendRequestAsync(HttpDataSource.MethodGet, HttpDataSourceResponseType.AsString);
            Assert.AreEqual(WaiterResults.Success, waiter.Wait(10, request), "Couldn't load data");
        }

        private int counter;

        [TestMethod]
        public void LoadHttpDataFollowedByMultipleCancel()
        {
            var request = new HttpDataSource("https://www.google.pl/search?q=a");
            var waiter = new AsyncWaiter();

            counter = 0;
            request.DataReceived += delegate { counter++; };
            request.DataReceiveFailed += delegate { counter++; };

            for (int i = 0; i < 10; i++)
            {
                request.SendRequestAsync(HttpDataSource.MethodGet, HttpDataSourceResponseType.AsString);

                waiter.Sleep(130);
                request.Cancel();
            }
            request.SendRequestAsync(HttpDataSource.MethodGet, HttpDataSourceResponseType.AsString);

            Assert.AreEqual(WaiterResults.Success, waiter.Wait(10, request), "Couldn't load data");
            Assert.AreEqual(1, counter, "Should receive data only once!");
        }
    }
}
