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
using CodeTitans.Core.Tasks;
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
    /// <summary>
    /// Summary description for TaskTests
    /// </summary>
    [TestClass]
    public class TaskTests
    {
        [TestMethod]
        public void InitializeEmptyExecutionQueue()
        {
            ITaskExecutor taskQueue = new ExecutionQueue("EQ");

            taskQueue.Start();

            if (!taskQueue.IsStarted)
                throw new InvalidOperationException();

            taskQueue.Stop(true);

            if (taskQueue.IsStarted)
                throw new InvalidOperationException();
        }

        [TestMethod]
        public void InitializeExecuteActions()
        {
            ITaskExecutor taskQueue = new ExecutionQueue();
            int counter = 0;

            taskQueue.Start();
            taskQueue.Add((e) => { counter++; });
            taskQueue.Add((e) => { if (counter == 1) counter++; });
            taskQueue.Add((e) => { if (counter == 2) counter++; });

            // complete all tasks:
            taskQueue.Stop(true);

            Assert.AreEqual(3, counter, "3 Tasks should perform their job!");
        }
    }
}
