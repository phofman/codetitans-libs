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
using System.Globalization;
using System.Reflection;
using CodeTitans.JSon;
using CodeTitans.NUnitTests;
using System.IO;
using System.Collections;
using System.Diagnostics;
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
    public class BSonWriterTests
    {
        [TestMethod]
        public void Create()
        {
            var writer = new BSonWriter();

            Assert.IsNotNull(writer);
        }

        [TestMethod]
        public void SampleDataSerialization()
        {
            var output = new byte[] { 0x31, 0x00, 0x00, 0x00, 0x04, 0x42, 0x53, 0x4F, 0x4E, 0x00, 0x26, 0x00, 0x00, 0x00, 0x02, 0x30, 0x00, 0x08, 0x00, 0x00, 0x00, 0x61, 0x77, 0x65, 0x73, 0x6F, 0x6D, 0x65, 0x00, 0x01, 0x31, 0x00, 0x33, 0x33, 0x33, 0x33, 0x33, 0x33, 0x14, 0x40, 0x10, 0x32, 0x00, 0xc2, 0x07, 0x00, 0x00, 0x00, 0x00 };
            var writer = new BSonWriter();

            using (writer.WriteObject())
            {
                writer.WriteMember("BSON");
                using (writer.WriteArray())
                {
                    writer.WriteValue("awesome");
                    writer.WriteValue(5.05d);
                    writer.WriteValue(1986);
                }
            }

            var result = writer.ToBytes();

            Assert.AreEqual(output.Length, result.Length);
            for (int i = 0; i < output.Length; i++)
                Assert.AreEqual(output[i], result[i]);
        }
    }
}
