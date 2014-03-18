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

        [TestMethod]
        public void FowardSelectedLogItems()
        {
            var listener1 = new CustomDebugListener();
            var listener2 = new CustomDebugListener();

            var forward = new ForwardDebugListener("Forward", listener1, new[] { "Cat1", "Cat2", "Cat3" });

            DebugLog.AddListener(forward);
            DebugLog.AddListener(listener2);

            DebugLog.WriteLine("Cat1.Send", "Message1"); // +1
            DebugLog.WriteLine("Cat2", "Message2"); // +1
            DebugLog.WriteLine("Cat3", "Message3"); // +1
            DebugLog.WriteLine("Cat3.XRunner", "Message3"); // +1
            DebugLog.WriteLine("C1", "Message4");
            DebugLog.WriteLine("Cat4", "Message5");
            DebugLog.WriteLine("Cat5.Cat1", "Message5");

            Assert.AreEqual(4, listener1.Count);
            Assert.AreEqual(7, listener2.Count);
        }

        [TestMethod]
        public void FilterSelectedLogItems()
        {
            var listener1 = new CustomDebugListener();
            var listener2 = new CustomDebugListener();

            var forward = new ForwardDebugListener("Forward", listener1, null, new[] { "Cat1", "Cat2", "Cat3" });

            DebugLog.AddListener(forward);
            DebugLog.AddListener(listener2);

            DebugLog.WriteLine("Cat1", "Message1");
            DebugLog.WriteLine("Cat1.Sub", "Message2");
            DebugLog.WriteLine("Cat3.Send", "Message3");
            DebugLog.WriteLine("AnySend", "Message4"); // +1
            DebugLog.WriteLine("Core", "Message5"); // +1

            Assert.AreEqual(2, listener1.Count);
            Assert.AreEqual(5, listener2.Count);

        }
    }
#endif
}
