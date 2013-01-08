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

using CodeTitans.Diagnostics;
#if NUNIT
using NUnit.Framework;
using TestClassAttribute=NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute=NUnit.Framework.TestAttribute;
using TestInitializeAttribute=NUnit.Framework.SetUpAttribute;
using TestCleanupAttribute=NUnit.Framework.TearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CodeTitans.UnitTests.Core
{
#if DEBUG
    [TestClass]
    public class DebugLogTests
    {
        private class CustomDebugListener : IDebugTraceListener
        {
            #region Implementation of IDebugTraceListener

            public string Name
            {
                get { return "CustomDebugListener"; }
            }

            public void WriteLine(DebugEntry entry)
            {
                Count++;
            }

            #endregion

            public int Count
            {
                get;
                private set;
            }
        }

        [TestMethod]
        public void WriteAnyLogs()
        {
            DebugLog.WriteAppLine("Line1");
            DebugLog.WriteBayeuxLine("Line2");
            DebugLog.WriteCoreLine("Line3");
            DebugLog.WriteLine("Category1", "Line4");
        }

        [TestMethod]
        public void WriteLogsAndDispose()
        {
            DebugLog.WriteCoreLine("Testing log item");
            DebugLog.Dispose();
        }

        [TestMethod]
        public void AddCustomDebugListener()
        {
            DebugLog.Listeners.Add(new CustomDebugListener());
        }

        [TestMethod]
        public void AddCustomDebugListenerAndWriteAnyLog()
        {
            var listener = new CustomDebugListener();
            DebugLog.Listeners.Add(listener);

            DebugLog.WriteAppLine("Test1");
            DebugLog.WriteBayeuxLine("Test2");
            DebugLog.WriteCoreLine("Test3");

            Assert.AreEqual(3, listener.Count, "Too few debug log entries received!");
        }

        [TestMethod]
        public void FilterLogItems()
        {
            var bayeuxListener = new FilterDebugListener("FilterBayeuxDebugListener", DebugLog.CategoryBayeux);
            DebugLog.Listeners.Add(bayeuxListener);

            DebugLog.WriteAppLine("Test1");
            DebugLog.WriteBayeuxLine("Test2");
            DebugLog.WriteCoreLine("Test3");

            Assert.AreEqual(1, bayeuxListener.Count, "Too many debug log entries received!");
        }
    }
#endif
}
