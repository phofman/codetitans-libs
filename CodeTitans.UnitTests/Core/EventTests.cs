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
using System.Threading;
using CodeTitans.Core;
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
    [TestClass]
    public class EventTests
    {
        private int _counter;

        private class NumericEventArgs : EventArgs
        {
            public NumericEventArgs(int value)
            {
                Value = value;
            }

            public int Value
            {
                get;
                private set;
            }
        }

        [TestMethod]
        public void UpdateCounterViaInvoke()
        {
            const int Value1 = 1;
            const int Value2 = 2;

            Assert.AreEqual(0, _counter, "Invalid counter value!");
            Event.Invoke(InternalCounterUpdate, this, new NumericEventArgs(Value1));
            Event.Invoke(InternalCounterUpdate, this, new NumericEventArgs(Value2));

            Assert.AreEqual(Value1 + Value2, _counter, "Unexpected counter value!");
        }

        private void InternalCounterUpdate(object sender, NumericEventArgs e)
        {
            if (e != null)
                _counter += e.Value;
            else
                _counter++;
        }

        [TestMethod]
        public void UpdateCounterViaDelayedInvoke()
        {
            const int Value1 = 1;
            const int Value2 = 2;
            const int Delay1 = 1000;
            const int Delay2 = 2000;

            Assert.AreEqual(0, _counter, "Invalid counter value!");
            Event.InvokeDelayed(Delay1, InternalCounterUpdate, this, new NumericEventArgs(Value1));
            Event.InvokeDelayed(Delay2, InternalCounterUpdate, this, new NumericEventArgs(Value2));

            Assert.AreEqual(0, _counter, "Too quickly increased counter value!");

            Thread.Sleep((Delay1 + Delay2) / 2);
            Assert.AreEqual(Value1, _counter, "Invalid value increased after Delay1!");
            Thread.Sleep((Delay1 + Delay2) / 2 + 100);
            Assert.AreEqual(Value1 + Value2, _counter, "Unexpected counter value!");
        }
    }
}
