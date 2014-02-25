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
using System.IO;
using CodeTitans.JSon;
#if NUNIT
using NUnit.Framework;
using TestClassAttribute=NUnit.Framework.TestFixtureAttribute;
using TestMethodAttribute=NUnit.Framework.TestAttribute;
using TestInitializeAttribute=NUnit.Framework.SetUpAttribute;
using TestCleanupAttribute=NUnit.Framework.TearDownAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace CodeTitans.UnitTests.JSon
{
    [TestClass]
    public class BSonReaderTests
    {
        [TestMethod]
        public void Create()
        {
            var reader = new BSonReader();

            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void ReadEmptyData()
        {
            var input = new byte[0];
            var reader = new BSonReader(input);

            var data = reader.Read();

            Assert.IsNull(data);
        }

        [TestMethod]
        public void ReadSimpleData()
        {
            // {"hello": "world"}
            var input = new byte[] { 0x16, 0x00, 0x00, 0x00, 0x02, 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x06, 0x00, 0x00, 0x00, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x00, 0x00 };
            var reader = new BSonReader(input);

            var data = reader.ReadAsJSonObject();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.IsEnumerable);

            Assert.IsTrue(data.Contains("hello"));
            Assert.AreEqual("world", data["hello"].StringValue);
        }

        [TestMethod]
        public void ReadSimpleDataViaStream()
        {
            // {"hello": "world"}
            var input = new byte[] { 0x16, 0x00, 0x00, 0x00, 0x02, 0x68, 0x65, 0x6C, 0x6C, 0x6F, 0x00, 0x06, 0x00, 0x00, 0x00, 0x77, 0x6F, 0x72, 0x6C, 0x64, 0x00, 0x00 };
            var reader = new BSonReader(new BinaryReader(new MemoryStream(input)));

            var data = reader.ReadAsJSonObject();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.IsEnumerable);

            Assert.IsTrue(data.Contains("hello"));
            Assert.AreEqual("world", data["hello"].StringValue);
        }

        [TestMethod]
        public void ReadSimpleData2()
        {
            // {"BSON": ["awesome", 5.05, 1986]}
            var input = new byte[] { 0x31, 0x00, 0x00, 0x00, 0x04, 0x42, 0x53, 0x4F, 0x4E, 0x00, 0x26, 0x00, 0x00, 0x00, 0x02, 0x30, 0x00, 0x08, 0x00, 0x00, 0x00, 0x61, 0x77, 0x65, 0x73, 0x6F, 0x6D, 0x65, 0x00, 0x01, 0x31, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x14, 0x40, 0x10, 0x32, 0x00, 0xc2, 0x07, 0x00, 0x00, 0x00, 0x00 };
            var reader = new BSonReader(input);

            var data = reader.ReadAsJSonObject();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.IsEnumerable);

            Assert.IsTrue(data.Contains("BSON"));
            var bsonArray = data["BSON"];
            Assert.IsTrue(bsonArray.IsEnumerable);
            Assert.AreEqual(3, bsonArray.Length);
            Assert.AreEqual("awesome", bsonArray[0].StringValue);
            Assert.AreEqual(5.05d, bsonArray[1].DoubleValue);
            Assert.AreEqual(1986, bsonArray[2].Int32Value);
        }

        [TestMethod]
        public void ReadSimpleData2ViaStream()
        {
            // {"BSON": ["awesome", 5.05, 1986]}
            var input = new byte[] { 0x31, 0x00, 0x00, 0x00, 0x04, 0x42, 0x53, 0x4F, 0x4E, 0x00, 0x26, 0x00, 0x00, 0x00, 0x02, 0x30, 0x00, 0x08, 0x00, 0x00, 0x00, 0x61, 0x77, 0x65, 0x73, 0x6F, 0x6D, 0x65, 0x00, 0x01, 0x31, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x14, 0x40, 0x10, 0x32, 0x00, 0xc2, 0x07, 0x00, 0x00, 0x00, 0x00 };
            var reader = new BSonReader(new BinaryReader(new MemoryStream(input)));

            var data = reader.ReadAsJSonObject();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.IsEnumerable);

            Assert.IsTrue(data.Contains("BSON"));
            var bsonArray = data["BSON"];
            Assert.IsTrue(bsonArray.IsEnumerable);
            Assert.AreEqual(3, bsonArray.Length);
            Assert.AreEqual("awesome", bsonArray[0].StringValue);
            Assert.AreEqual(5.05d, bsonArray[1].DoubleValue);
            Assert.AreEqual(1986, bsonArray[2].Int32Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FailToReadMissingData()
        {
            var reader = new BSonReader();

            // should throw an exception...
            reader.ReadAsJSonMutableObject();
        }
    }
}
