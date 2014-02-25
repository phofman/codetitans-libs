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

using System.IO;
using System.Text;
using CodeTitans.Helpers;
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
    public class BinaryReaderTests
    {
        [TestMethod]
        public void CreateArrayReader()
        {
            var reader = BinaryHelper.CreateReader((byte[]) null);

            Assert.IsNotNull(reader);
            Assert.IsTrue(reader.IsEmpty);
            Assert.IsFalse(reader.IsEof);

            Assert.IsNotNull(reader);
        }

        [TestMethod]
        public void ReadSimpleData()
        {
            var reader = BinaryHelper.CreateReader(new byte[] {1, 0, 0, 0, 2});

            Assert.IsNotNull(reader);
            Assert.AreEqual(1u, reader.ReadUInt32());
            Assert.AreEqual((byte)2, reader.ReadByte());
        }
    }
}
