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
using CodeTitans.Core;
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
    public class RangeTests
    {
        [TestMethod]
        public void SerializeRange()
        {
            Range r = new Range(10, 1);

            Assert.AreEqual(r.ToString(), "{10, 1}");
            Assert.AreNotEqual(r.ToString(), "{10,1}");
            Assert.AreEqual(r.ToString("P"), "(10, 1)");
            Assert.AreEqual(r.ToString("S"), "[10, 1]");

            Range q = new Range(7, 23);

            Assert.AreEqual(q.ToString(), "{7, 23}");
            Assert.AreNotEqual(q.ToString(), "{7,1}");
            Assert.AreEqual(q.ToString("P"), "(7, 23)");
            Assert.AreEqual(q.ToString("S"), "[7, 23]");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseNullRange()
        {
            Range.Parse(null);
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseEmptyRange()
        {
            Range.Parse("");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange1()
        {
            Range.Parse("10:");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange2()
        {
            Range.Parse("(10 1");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange3()
        {
            Range.Parse("(10 1]");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange4()
        {
            Range.Parse("(10,,1)");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange5()
        {
            Range.Parse("(10 1) [1,2]");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange6()
        {
            Range.Parse("{10!1}");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange7()
        {
            Range.Parse("{10::1}");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ParseInvalidRange8()
        {
            Range.Parse(" {  10 - 1 } ");
            Assert.Fail("Parsing should throw an exception");
        }

        [TestMethod]
        public void ParseRange1()
        {
            var r = Range.Parse("10:1");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(10, 1));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange2()
        {
            var r = Range.Parse("10: 1");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(10, 1));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange3()
        {
            var r = Range.Parse("  10  :  1  ");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(10, 1));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange4()
        {
            var r = Range.Parse("{10 1}");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(10, 1));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange5()
        {
            var r = Range.Parse("{6,+112}");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(6, 112));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange6()
        {
            var r = Range.Parse("(-6,+112)");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(-6, 112));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange7()
        {
            var r = Range.Parse("{-6, 12}");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(-6, 12));
            Assert.AreNotEqual(r, new Range(1, 10));
        }

        [TestMethod]
        public void ParseRange8()
        {
            var r = Range.Parse("{-6:12}");

            Assert.IsNotNull(r);
            Assert.AreEqual(r, new Range(-6, 12));
            Assert.AreNotEqual(r, new Range(1, 10));
        }
    }
}
