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
    public class FieldsParsingTests
    {
        [TestMethod]
        public void ParseErrorMessageOnly()
        {
            const string message = "This is message only";
            BayeuxError error = new BayeuxError(message);

            Assert.AreEqual(error.Message, message, "Invalid message");
            Assert.IsNotNull(error.Arguments, "Argumets should not be null");
            Assert.AreEqual(error.Arguments.Count, 0, "Invalid number or message arguments");
            Assert.AreEqual(error.Code, 0, "Invalid error code");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseErrorMessageWithoutCode()
        {
            const string message = "::No clientID";
            BayeuxError error = new BayeuxError(message);

            Assert.IsTrue(false, "Exception should be thrown already!");
        }

        [TestMethod]
        public void ParseErrorMessageWithCode()
        {
            const string message = "No clientID";
            BayeuxError error = new BayeuxError("101::" + message);

            Assert.AreEqual(error.Message, message, "Invalid message");
            Assert.IsNotNull(error.Arguments, "Argumets should not be null");
            Assert.AreEqual(error.Arguments.Count, 0, "Invalid number or message arguments");
            Assert.AreEqual(error.Code, 101, "Invalid error code");
        }

        [TestMethod]
        public void ParseErrorWithoutMessage()
        {
            const string message = "";
            BayeuxError error = new BayeuxError("101::" + message);

            Assert.AreEqual(error.Message, message, "Invalid message");
            Assert.IsNotNull(error.Arguments, "Argumets should not be null");
            Assert.AreEqual(error.Arguments.Count, 0, "Invalid number or message arguments");
            Assert.AreEqual(error.Code, 101, "Invalid error code");
        }

        [TestMethod]
        public void ParseErrorWithArgs()
        {
            const string message = "No Message";
            const string argument = "/foo/bar";
            BayeuxError error = new BayeuxError("101:" + argument + ":" + message);

            Assert.AreEqual(error.Message, message, "Invalid message");
            Assert.IsNotNull(error.Arguments, "Argumets should not be null");
            Assert.AreEqual(error.Arguments.Count, 1, "Invalid number or message arguments");
            Assert.AreEqual(error.Arguments[0], argument, "Invalid argument");
            Assert.AreEqual(error.Code, 101, "Invalid error code");
        }
    }
}
