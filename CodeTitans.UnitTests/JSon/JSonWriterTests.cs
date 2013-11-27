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
using System.Text;
using System.Collections.Generic;
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
    public class JSonWriterTests
    {
        private JSonWriter writer;
        private JSonWriter writerNoIndent;

        [TestInitialize]
        public void TestInit()
        {
            writer = new JSonWriter(true);
            writerNoIndent = new JSonWriter(false);
        }

        [TestCleanup]
        public void TestClean()
        {
            Console.WriteLine("Serialized JSON object:");
            Console.WriteLine(writer.ToString());
            writer.Close();
        }

        [TestMethod]
        public void WriteAnObject()
        {
            writer.WriteObjectBegin();
            writer.WriteObjectEnd();

            Console.WriteLine(writer);
            Assert.AreEqual("{}", writer.ToString(), "Empty object expected!");
        }

        [TestMethod]
        public void WriteAnArray()
        {
            writer.WriteArrayBegin();
            writer.WriteArrayEnd();

            Assert.AreEqual("[]", writer.ToString(), "Empty array expected!");
        }

        [TestMethod]
        public void WriteAnArrayUsingSimplifiedSyntax()
        {
            using(writer.WriteArray())
            {
            }

            Assert.AreEqual("[]", writer.ToString(), "Empty array expected!");
        }

        [TestMethod]
        public void WriteEmbeddedArrayUsingSimplifiedSyntax()
        {
            using (writer.WriteArray())
            {
                using (writer.WriteArray())
                {

                }
            }

            Assert.AreEqual("[\r\n    []\r\n]", writer.ToString(), "Non-empty array expected!");
        }

        [TestMethod]
        public void WriteEmbeddedArrayWithItemsUsingSimplifiedSyntax()
        {
            using (var a = writer.WriteArray())
            {
                a.WriteValue("Hello");
                a.WriteValue(1);
                a.WriteValue('b');

                using (var b = writer.WriteArray())
                {
                    b.WriteValue("XXX");
                }
            }

            Assert.AreEqual("[\r\n    \"Hello\",\r\n    1,\r\n    98,\r\n    [\r\n        \"XXX\"\r\n    ]\r\n]", writer.ToString(), "Non-empty array expected!");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonWriterException))]
        public void WriteInvalidMember()
        {
            writer.WriteMember("name", "value");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonWriterException))]
        public void WriteImproperObjectEnd()
        {
            writer.WriteObjectBegin();
            writer.WriteArrayEnd();
        }

        [TestMethod]
        public void WriteEmbeddedObject()
        {
            writer.WriteArrayBegin();
            writer.WriteObjectBegin();
            writer.WriteMember("Name", "Paweł");
            writer.WriteMember("Salary", 100);
            writer.WriteMember("Company");
            writer.WriteObjectBegin();
            writer.WriteMember("Name", "CodeTitans");
            writer.WriteMember("Address", "ABCD");
            writer.WriteMember("Value", 10.28437411e2);
            writer.WriteMember("Started", DateTime.Now);
            writer.WriteMemberNull("Account");
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();

            writer.WriteObjectBegin();
            writer.WriteMember("Name", "Aga");
            writer.WriteMember("Salary", 200);
            writer.WriteObjectEnd();

            writer.WriteValue(101);
            writer.WriteValue("New Item");

            writer.WriteArrayEnd();
        }

        [TestMethod]
        public void WriteEmbeddedObjectUsingSimplifiedSyntax()
        {
            using (writer.WriteArray())
            {
                using (writer.WriteObject())
                {
                    writer.WriteMember("Name", "Paweł");
                    writer.WriteMember("Salary", 100);
                    using (writer.WriteObject("Company"))
                    {
                        writer.WriteMember("Name", "CodeTitans");
                        writer.WriteMember("Address", "ABCD");
                        writer.WriteMember("Value", 10.28437411e2);
                        writer.WriteMember("Started", DateTime.Now);
                        writer.WriteMemberNull("Account");
                    }
                }

                using (writer.WriteObject())
                {
                    writer.WriteMember("Name", "Aga");
                    writer.WriteMember("Salary", 200);
                }

                writer.WriteValue(101);
                writer.WriteValue("New Item");
            }
        }

        [TestMethod]
        public void WriteArrayOfString()
        {
            writer.Indent = false;
            writer.Write(new string[] { "small", "medium", "large", "huge" });
        }

        [TestMethod]
        public void WriteArrayOfBytes()
        {
            writer.Indent = false;
            writer.Write(new byte[] { 1, 2, 3, 4, 5, 6, byte.MaxValue, byte.MinValue });
        }

        [TestMethod]
        public void WriteArrayOfInt64()
        {
            writer.Indent = false;
            writer.Write(new Int64[] { Int64.MinValue, 0, 1, 2, 3, Int64.MaxValue });
        }

        [TestMethod]
        public void WriteArrayOfUInt64()
        {
            writer.Indent = false;
            writer.Write(new UInt64[] { UInt64.MinValue, 0, 1, 2, 3, UInt64.MaxValue });
        }

        [TestMethod]
        public void WriteValueWithoutObject()
        {
            writer.WriteValue("AAAA");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonWriterException))]
        public void WriteSeveralValuesWithoutArray()
        {
            writer.WriteValue("AAA");
            writer.WriteValue("BBB");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonWriterException))]
        public void WriteValueInsideObjectInvalid()
        {
            writer.WriteObjectBegin();
            writer.WriteValue("AAA");
            writer.WriteObjectEnd();
        }

        [TestMethod]
        public void TestSerializeString()
        {
            writer.Write("Paweł");
        }

        [TestMethod]
        public void WriteArrayOfInt32()
        {
            writer.WriteArrayBegin();
            writer.WriteValue(123);
            writer.WriteValue(234);
            writer.WriteValue(345);
            writer.WriteValue(456);
            writer.WriteArrayEnd();
        }

        [TestMethod]
        public void WriteQuotationCheck1()
        {
            writer.Indent = false;
            writer.WriteObjectBegin();
            writer.WriteMember("Name", 123);
            writer.WriteObjectEnd();

            Assert.AreEqual("{\"Name\": 123}", writer.ToString(), "Expected formatted object with one field!");
        }

        [TestMethod]
        public void WriteQuotationCheck2()
        {
            writer.Indent = false;
            writer.WriteArrayBegin();
            writer.WriteValue("ABC");
            writer.WriteArrayEnd();

            Assert.AreEqual("[\"ABC\"]", writer.ToString(), "Expected array with one quoted element!");
        }

        [TestMethod]
        public void WriteInternalArray()
        {
            writer.WriteObjectBegin();
            writer.WriteMember("Name", "Paweł");
            writer.WriteMember("Accounts");
            writer.WriteArrayBegin();
            writer.WriteValue("111-222-333");
            writer.WriteValue("222-333-444");
            writer.WriteValue("333-444-555");
            writer.WriteArrayEnd();
            writer.WriteMember("Salary", 100);
            writer.WriteMember("Friends");
            writer.WriteArrayBegin();
            writer.WriteObjectBegin();
            writer.WriteMember("Name", "Aga");
            writer.WriteMember("Ref", 12345);
            writer.WriteObjectEnd();
            writer.WriteObjectBegin();
            writer.WriteMember("Name", "Ala");
            writer.WriteMember("Ref", 23456);
            writer.WriteObjectEnd();
            writer.WriteArrayEnd();
            writer.WriteMember("Company");
            writer.WriteObjectBegin();
            writer.WriteMember("Name", "CodeTitans");
            writer.WriteMember("Address", (string)null);
            writer.WriteMember("CreatedAt", DateTime.Now);
            writer.WriteObjectEnd();
            writer.WriteObjectEnd();

            Assert.IsTrue(writer.IsValid);
        }

        [TestMethod]
        public void WriteObjectMember()
        {
            writer.Indent = false;
            writer.WriteObjectBegin();
            writer.WriteMember("Name");
            writer.WriteValue("Paweł");
            writer.WriteObjectEnd();

            Assert.AreEqual("{\"Name\": \"Paweł\"}", writer.ToString(), "Expected object with one member!");
        }

        [TestMethod]
        public void WriteTimeSpan()
        {
            writer.WriteArrayBegin();
            writer.WriteValue(new TimeSpan(1, 1, 10));
            writer.WriteValue(new TimeSpan(100000));
            writer.WriteArrayEnd();
        }

        [TestMethod]
        public void WriteGuid()
        {
            writer.WriteArrayBegin();
            writer.WriteValue(Guid.NewGuid());
            writer.WriteValue(Guid.NewGuid());
            writer.WriteValue(Guid.NewGuid());
            writer.WriteValue(Guid.NewGuid());
            writer.WriteArrayEnd();
        }

        [TestMethod]
        public void WriteAutoArray()
        {
            var array = new[] { "Paweł", "Alicja", "Gaweł", "Agnieszka" };

            writer.Indent = false;
            writer.Write(array);

            Assert.AreEqual("[\"Paweł\", \"Alicja\", \"Gaweł\", \"Agnieszka\"]", writer.ToString(), "Expected object as array value!");
        }

        [TestMethod]
        public void WriteAutoDictArray()
        {
            var data = new Dictionary<string, object>();
            var complex = new Dictionary<string, string>();

            complex.Add("Name", "Piotr");
            complex.Add("id", "id-abcd");

            data.Add("Name", "Paweł");
            data.Add("Age", 29);
            data.Add("Salary", 123.456);
            data.Add("BirthDate", new DateTime(1981, 8, 18));
            data.Add("Relatives", new object[] { "Agnieszka", "Alicja", "Zdzisław", "Wiesława", complex });

            writer.Indent = false;
            writer.Write(data);

            Assert.AreEqual("{\"Name\": \"Paweł\", \"Age\": 29, \"Salary\": 123.456, \"BirthDate\": \"1981-08-17 22:00:00Z\", \"Relatives\": [\"Agnieszka\", \"Alicja\", \"Zdzisław\", \"Wiesława\", {\"Name\": \"Piotr\", \"id\": \"id-abcd\"}]}", writer.ToString(), "Expected formatted object!");
        }

        [TestMethod]
        public void WriteComplexData()
        {
            writer.WriteObjectBegin();

            writer.WriteMember("id", 1);
            writer.WriteMember("method", "slim.request");
            writer.WriteMember("params");

            writer.WriteArrayBegin();
            writer.WriteValue("-");
            writer.WriteArrayBegin();
            writer.WriteValue("player");
            writer.WriteValue("count");
            writer.WriteValue("?");
            writer.WriteArrayEnd();
            writer.WriteArrayEnd();

            writer.WriteObjectEnd();
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ToStringWithInvalidFormat()
        {
            var reader = new JSonReader();
            var data = reader.ReadAsJSonObject("[1, 2, 3, [\"a\", \"b\", \"c\", [4, 5, 6, [{\"f\": 1, \"x\": 2}]]]]");

            Assert.IsNotNull(data);
            data.ToString("xxl");
        }

        [TestMethod]
        public void ToStringFormatWithIndentedOutput()
        {
            var reader = new JSonReader();
            var data = reader.ReadAsJSonObject("[1, 2, 3, [\"a\", \"b\", \"c\", [4, 5, 6, [{\"f\": 1, \"x\": 2}]]]]");

            Assert.IsNotNull(data);
            var outputIndent = data.ToString("i");
            Assert.IsNotNull(outputIndent);

            var outputCompact = data.ToString("c");
            Assert.IsNotNull(outputCompact);

            Assert.IsTrue(outputIndent.Length > outputCompact.Length);
            Assert.IsTrue(outputIndent.IndexOf("    ") > 0);
        }

        [TestMethod]
        public void ToStringFormatWithCompactedOutput()
        {
            var reader = new JSonReader();
            var data = reader.ReadAsJSonObject("[1,2,3,[\"a\",\"b\",\"c\",[4,5,6,[{\"f\":1,\"x\":2}]]]]");

            Assert.IsNotNull(data);
            var outputIndent = data.ToString("i");
            Assert.IsNotNull(outputIndent);

            var outputCompact = data.ToString("c");
            Assert.IsNotNull(outputCompact);

            Assert.IsTrue(outputIndent.Length > outputCompact.Length);
            Assert.IsTrue(outputCompact.IndexOf(' ') == -1);
        }

        [TestMethod]
        public void ToStringFormatWithNormalOutput()
        {
            var reader = new JSonReader();
            var data = reader.ReadAsJSonObject("[1,2,3,[\"a\",\"b\",\"c\",[4,5,6,[{\"f\":1,\"x\":2}]]]]");

            Assert.IsNotNull(data);
            var outputIndent = data.ToString("i");
            Assert.IsNotNull(outputIndent);

            var outputNormal = data.ToString("n");
            Assert.IsNotNull(outputNormal);

            Assert.IsTrue(outputIndent.Length > outputNormal.Length);

            Assert.IsTrue(outputNormal.IndexOf(' ') >= 0);
            outputNormal = outputNormal.Replace(", ", ",").Replace(": ", ":");
            Assert.IsTrue(outputNormal.IndexOf(' ') == -1);
        }

        [TestMethod]
        public void SerializeJavaScriptDate1970()
        {
            using (writerNoIndent.WriteArray())
            {
                writerNoIndent.WriteValue(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), JSonWriterDateFormat.JavaScript);
            }

            Assert.AreEqual("[\"\\/Date(0)\\/\"]", writerNoIndent.ToString());
        }

        [TestMethod]
        public void SerializeDateBefore1970InJavaScriptFormat()
        {
            using (writerNoIndent.WriteArray())
            {
                writerNoIndent.WriteValue(new DateTime(1960, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), JSonWriterDateFormat.JavaScript);
            }

            Assert.AreEqual("[\"\\/Date(1960,1,1)\\/\"]", writerNoIndent.ToString());
        }

        [TestMethod]
        public void SerializeDateBefore1970ButWithSecondsInJavaScriptFormat()
        {
            using (writerNoIndent.WriteArray())
            {
                writerNoIndent.WriteValue(new DateTime(1950, 7, 18, 0, 11, 22, 33, DateTimeKind.Utc), JSonWriterDateFormat.JavaScript);
            }

            Assert.AreEqual("[\"\\/Date(1950,7,18,0,11,22,33)\\/\"]", writerNoIndent.ToString());
        }
    }
}
