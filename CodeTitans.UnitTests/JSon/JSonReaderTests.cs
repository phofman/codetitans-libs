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
    public class JSonReaderTests
    {
        [TestInitialize]
        public void TestInit()
        {
        }

        [TestMethod]
        public void ParseMultipartWithSuccess()
        {
            var reader = new JSonReader(true);

            var data1 = reader.Read("[1][2]");

            Assert.IsNotNull(data1);
            Assert.AreEqual(1, (long)((object[])data1)[0]);

            var data2 = reader.Read();
            Assert.IsNotNull(data2);
            Assert.AreEqual(2, (long)((object[])data2)[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseForbiddenMultipartWithException()
        {
            var reader = new JSonReader();

            reader.Read("[0][1]");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadWithoutSourceSetWithException()
        {
            var reader = new JSonReader();

            reader.Read();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ReadObjectWithoutSourceSetWithException()
        {
            var reader = new JSonReader();

            reader.ReadAsJSonObject();
        }

        [TestMethod]
        public void ParseToJSon()
        {
            const string jsonText = "[{\"minimumVersion\":\"1.0\",\"type\": null,\"channel\":\"\\/meta\\/handshake\",\"supportedConnectionTypes\":[\"request-response\"],\"successful\":true}]";

            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonObject();
            Assert.AreEqual("/meta/handshake", result[0]["channel"].StringValue, "Invalid channel");
            Assert.IsTrue(result[0]["type"].IsNull, "Type should be null");
            Assert.AreEqual(1, result[0]["supportedConnectionTypes"].Count, "Supported types are an array with 1 element");
            Assert.AreEqual(1.0, result[0]["minimumVersion"].DoubleValue, "Current version is 1.0");
            Assert.IsTrue(result[0]["successful"].IsTrue, "Operation finished with success!");
        }

        [TestMethod]
        public void ParseEmptyJSonString()
        {
            var reader = new JSonReader("  ");
            var result = reader.Read();

            Assert.IsNull(result, "Result should be null");
        }

        [TestMethod]
        public void ParseEmptyArray()
        {
            var reader = new JSonReader("  [    ]");
            var result = reader.Read();

            AssertExt.IsInstanceOf<Array>(result, "Result should be an array");
        }

        [TestMethod]
        [ExpectedException(typeof (JSonReaderException))]
        public void ParseWrongOpenArray()
        {
            try
            {
                var reader = new JSonReader("  [   ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(2, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseWrongCloseObject()
        {
            try
            {
                var reader = new JSonReader("}");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(0, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseWrongCloseArray()
        {
            try
            {
                var reader = new JSonReader("  \t\t\n \t\t]   ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(3, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseUnfinishedString()
        {
            try
            {
                var reader = new JSonReader("   \"start");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(3, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseWrongNumberOfMainElements()
        {
            try
            {
                var reader = new JSonReader("  [ ], [], { }  ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(5, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseArrayWithoutComma()
        {
            try
            {
                var reader = new JSonReader(" [ \"a\",\r\n\r\n \"b\"\r\n \t   \"c\" ]");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(3, ex.Line);
                Assert.AreEqual(5, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseArrayWithoutCommaNextItemOnLineStart()
        {
            try
            {
                var reader = new JSonReader(" [ \"a\"\r\n\r\n\"b\"]");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(2, ex.Line);
                Assert.AreEqual(0, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseTopLevelElementsWithoutComma()
        {
            try
            {
                var reader = new JSonReader("  [ ]\r\n [] { }  ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(1, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseArrayOfBytes()
        {
            var reader = new JSonReader("[1, 2, 3, 4, 5, 6, 255, 0]");
            var result = reader.Read();
            Assert.IsNotNull(result);
            AssertExt.IsInstanceOf<Array>(result, "Result should be an Array");

            object[] array = (object[]) result;
            Assert.AreEqual((long) 1, array[0], "First element should be equal to 1");

            // try the same with JSonObject:
            var oResult = reader.ReadAsJSonObject("[1, 2, 3, 4, 5, 6, 255, 0]");
            Assert.IsNotNull(oResult);
            Assert.AreEqual(1, oResult[0].Int64Value, "First element should be equal to 1");
        }

        [TestMethod]
        public void ParseArrayOfUInt64()
        {
            var reader = new JSonReader("[0, 0, 1, 2, 3, 18446744073709551615]");
            var result = reader.Read();
            Assert.IsNotNull(result);
            AssertExt.IsInstanceOf<Array>(result, "Result should be an Array");

            object[] array = (object[]) result;
            Assert.AreEqual((object) ulong.MaxValue, array[5], "First element should be equal to ulong.MaxValue");

            // try the same with JSonObject:
            var oResult = reader.ReadAsJSonObject("[0, 0, 1, 2, 3, 18446744073709551615]");
            Assert.IsNotNull(oResult);
            Assert.AreEqual((ulong) ulong.MaxValue, oResult[5].UInt64Value, "First element should be equal to ulong.MaxValue");
        }

        [TestMethod]
        public void ParseKeyword()
        {
            var reader = new JSonReader("  null  ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<DBNull>(result, "Item should be null");
        }

        [TestMethod]
        public void ParseNumber()
        {
            var reader = new JSonReader("  10.1e-2  ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<double>(result, "Item should be a number");
            Assert.AreEqual(result, 10.1e-2);
        }

        [TestMethod]
        public void ParseNumberInt64()
        {
            var reader = new JSonReader("  " + Int64.MaxValue + "  ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<Int64>(result, "Item should be a number");
            Assert.AreEqual(Int64.MaxValue, result);
        }

        [TestMethod]
        public void ParseNumberTicks()
        {
            DateTime now = DateTime.Now;
            var reader = new JSonReader("  " + now.Ticks + "  ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<Int64>(result, "Item should be a number");
            Assert.AreEqual(now.Ticks, result);
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidNumber()
        {
            try
            {
                var reader = new JSonReader("  10,1e-2  ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(4, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseNullKeyword()
        {
            var reader = new JSonReader("   null");
            var result = reader.ReadAsJSonObject();

            Assert.AreEqual(result.StringValue, null, "Expected a null string!");
        }

        [TestMethod]
        public void ParseTrueKeyword()
        {
            var reader = new JSonReader("  true  ");
            var result = reader.ReadAsJSonObject();

            Assert.AreEqual("true", result.StringValue, "Expected 'true' string returned");
            Assert.AreEqual(true, result.BooleanValue, "Expected boolean true value");
            Assert.AreEqual(1, result.DoubleValue, "Expected non zero value");
        }

        [TestMethod]
        public void ParseString()
        {
            var reader = new JSonReader("  \"Paweł\\r\\nJSON\\r\\nText\ttab\tspaced.\"  ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<string>(result, "Item should be a string");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidNotFinishedString()
        {
            try
            {
                var reader = new JSonReader("\n\"Not finished text  ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(0, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidStringWithNewLine()
        {
            try
            {
                var reader = new JSonReader("  \"Test string with new line\r\n\"");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(3, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseCorrectlyTooLongUnicodeCharacterDefinitionInString()
        {
            var reader = new JSonReader("  \"Text-\\u12345\"");
            var result = reader.Read();

            Assert.IsNotNull(result, "Result can't be null");
            Assert.AreEqual("Text-\u12345", result, "Invalid text read");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidUnicodeCharacters()
        {
            try
            {
                var reader = new JSonReader("\"test-\\u12\\u123\"");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(6, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseSeveralUnicodeCharacters()
        {
            var reader = new JSonReader("\"\\u308f\\u3084\\u307e\\u3061\\u306a\\u306a\"");
            var result = reader.Read();

            Assert.IsNotNull(result, "Value should be some string");
            AssertExt.IsInstanceOf<string>(result, "Value shoule be string class");
            Assert.AreEqual("\u308f\u3084\u307e\u3061\u306a\u306a", result);
        }

        [TestMethod]
        public void ParseValidUnicodeCharacterInString()
        {
            var reader = new JSonReader("  \"\\u0020\"");
            var result = reader.Read();

            AssertExt.IsInstanceOf<string>(result, "Item should be string");
            Assert.AreEqual(" ", result, "Single space was provided as input!");
        }

        [TestMethod]
        public void ParseStringSpecialCharacter()
        {
            var reader = new JSonReader("  \"\\\"\"");
            var result = reader.Read();

            AssertExt.IsInstanceOf<string>(result, "Item should be string");
            Assert.AreEqual("\"", result, "Expected special character!");
        }

        [TestMethod]
        public void ParseArray()
        {
            var reader = new JSonReader(" [ \"a\", \"b\", \"c\" ]");
            var result = reader.Read();

            AssertExt.IsInstanceOf<Array>(result, "Item should be an array");
            Assert.AreEqual(3, ((Array) result).Length, "Invalid number of items");
            Assert.AreEqual("a", ((object[]) result)[0], "Invalid first element");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseEmptyArrayWithComma()
        {
            try
            {
                var reader = new JSonReader(" [,] ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(2, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseEmptyObject()
        {
            var reader = new JSonReader("  { } ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<IDictionary>(result, "Item should be a dictionary");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseEmptyObjectWithComma()
        {
            try
            {
                var reader = new JSonReader("\n{,} ");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(1, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseArrayWithUnknownKeyword()
        {
            try
            {
                var reader = new JSonReader("\n \t[nulll]");
                reader.Read();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(1, ex.Line);
                Assert.AreEqual(3, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseSimpleData1()
        {
            var reader = new JSonReader("  { \"A\":1, \"B\": 2   , \"c\": 12.1 } ");
            var result = reader.Read();

            AssertExt.IsInstanceOf<IDictionary>(result, "Item should be a dictionary");
        }

        private string LoadTestInputFile(string name)
        {
            using (Stream st = Assembly.GetExecutingAssembly().GetManifestResourceStream("CodeTitans.NUnitTests.Resources." + name))
            {
                if (st != null)
                {
                    using (TextReader tr = new StreamReader(st))
                    {
                        return tr.ReadToEnd();
                    }
                }
            }

            using (var st = Assembly.GetExecutingAssembly().GetManifestResourceStream("CodeTitans.UnitTests.Resources." + name))
            {
                if (st != null)
                {
                    using (var tr = new StreamReader(st))
                        return tr.ReadToEnd();
                }
            }

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            var text = File.Exists(path + name) ? File.ReadAllText(path + name) : File.ReadAllText(path + "Resources\\" + name);
            return text;
        }

        [TestMethod]
        public void ParseAdvancedStructure()
        {
            var jsonText = LoadTestInputFile("advanced.json");
            var watchReader = Stopwatch.StartNew();
            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonObject();
            watchReader.Stop();

            var watchWriter = Stopwatch.StartNew();
            var writer = new JSonWriter(true);
            writer.Write(result);
            watchWriter.Stop();
            Console.WriteLine("Parsing taken: {0}", watchReader.Elapsed);
            Console.WriteLine("Serializing taken: {0}", watchWriter.Elapsed);
            Console.WriteLine("{0}", writer.ToString());

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ParseAdvancedGuidArray()
        {
            var jsonText = LoadTestInputFile("guid.json");
            var watchReader = Stopwatch.StartNew();
            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonObject();
            watchReader.Stop();

            Assert.IsNotNull(result);
            AssertExt.IsInstanceOf<Guid>(result[0].GuidValue, "Read element should be Guid");
            Console.WriteLine("Parsing taken: {0}", watchReader.Elapsed);
        }

        [TestMethod]
        public void ParseBayeuxConnectResponse()
        {
            var reader = new JSonReader("[{\"channel\":\"/meta/connect\",\"advice\":{\"reconnect\":\"retry\",\"interval\":0,\"timeout\":20000},\"successful\":true,\"id\":\"1\"}]");
            var result = reader.ReadAsJSonObject();

            Assert.IsNotNull(result, "Invalid parsed object!");

            int timeout = result[0]["advice"]["timeout"].Int32Value;
            Assert.AreEqual(20000, timeout, "Invalid timeout value!");
        }

        [TestMethod]
        public void DefaultValues()
        {
            var reader = new JSonReader("{ }");
            var result = reader.ReadAsJSonObject();

            // result is an empty object, so all the time the default values should be used:
            Assert.AreEqual(result["test", true].BooleanValue, true);

            Assert.AreEqual(result["url", "{ \"image\": \"default.png\" }", true]["image", null, false].StringValue, "default.png");

            DateTime now = DateTime.Now;

            // remove below milliseconds:
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, now.Kind);

            Assert.AreEqual(now, result["date", now].DateTimeValue);

            Assert.AreEqual(double.MaxValue, result["double", double.MaxValue].DoubleValue);
            Assert.AreEqual(Int64.MaxValue, result["int", Int64.MaxValue].Int64Value);
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void MissingCommaInObjectDefinition()
        {
            const string jsonText = @"{ ""field1"": ""value1"" ""field2"": ""value2"" }";

            try
            {
                var reader = new JSonReader(jsonText);
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(21, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void TwoCommasInObjectDefinition()
        {
            const string jsonText = @"{ ""field1"": ""value1"",, ""field2"": ""value2"" }";

            try
            {
                var reader = new JSonReader(jsonText);
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(21, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void CommaInTheEndOfObjectDefinitionAndNothingMoreAfter()
        {
            const string jsonText = @"{ ""field1"": ""value1"", ""field2"": ""value2"", }";

            try
            {
                var reader = new JSonReader(jsonText);
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(42, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void KeywordAsInvalidObjectFieldName()
        {
            const string jsonText = @"{ null: ""value1"" }";

            try
            {
                var reader = new JSonReader(jsonText);
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(2, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void MissingCommaInArrayDefinition()
        {
            const string jsonText = @"[ ""value1"" ""value2"" ""value3"" }";

            try
            {
                var reader = new JSonReader(jsonText);
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(11, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseDataTimeUtc()
        {
            DateTime date = new DateTime(2012, 5, 14, 11, 22, 33, DateTimeKind.Utc);

            var reader = new JSonReader(String.Format("\"{0}\"", date.ToString("u")));
            var result = reader.ReadAsJSonObject().DateTimeValue.ToUniversalTime();

            AssertExt.IsInstanceOf<DateTime>(result, "Item should be a DataTime");
            Assert.AreEqual(result, date);
        }

        [TestMethod]
        public void ParseDecimalGivenAsString()
        {
            var reader = new JSonReader("\"12.3456\"");
            var result = reader.ReadAsJSonObject().DecimalValue;

            AssertExt.IsInstanceOf<decimal>(result, "Item should be a number");
            Assert.AreEqual(result, new Decimal(12.3456));
        }

        [TestMethod]
        public void ParseDecimalGivenAsString2()
        {
            var reader = new JSonReader("\"12312.3456\"");
            var result = reader.ReadAsJSonObject().DecimalValue;

            AssertExt.IsInstanceOf<decimal>(result, "Item should be a number");
            Assert.AreEqual(result, new Decimal(12312.3456));
        }

        [TestMethod]
        public void ParseNumberWithDecimalForced()
        {
            var reader = new JSonReader("[12.12, 13.13, 14.14]");
            var result = reader.ReadAsJSonObject(JSonReaderNumberFormat.AsDecimal);
            var result0 = result[0].DecimalValue;
            var result1 = result[1].Int32Value;
            var result1Decimal = result[1].DecimalValue;

            AssertExt.IsInstanceOf<decimal>(result0, "Item should be a number");
            Assert.AreEqual(result0, new Decimal(12.12));

            AssertExt.IsInstanceOf<int>(result1, "Item should be a number");
            Assert.AreEqual(result1, 13);
            AssertExt.IsInstanceOf<decimal>(result1Decimal, "Item should be a number");
            Assert.AreEqual(result1Decimal, new Decimal(13.13));
        }

        [TestMethod]
        public void ParseNumberWithForcedFormat()
        {
            var reader = new JSonReader();
            
            var result1 = reader.ReadAsJSonObject("[12, 13, 14]", JSonReaderNumberFormat.AsInt32);
            var result2 = reader.ReadAsJSonObject("[" + (uint.MaxValue + 1ul) + ", " + (uint.MaxValue + 2ul) + ", " + (uint.MaxValue + 3ul) + "]", JSonReaderNumberFormat.AsInt64);
            var result3 = reader.ReadAsJSonObject("[12.12, 13.13, 14.14]", JSonReaderNumberFormat.AsDouble);
            var result4 = reader.ReadAsJSonObject("[12.12, 13.13, 14.14]", JSonReaderNumberFormat.AsDecimal);
            var result5 = reader.Read("[12, 13, 14]", JSonReaderNumberFormat.AsInt32);
            var result6 = reader.Read("[" + (uint.MaxValue + 1ul) + ", " + (uint.MaxValue + 2ul) + ", " + (uint.MaxValue + 3ul) + "]", JSonReaderNumberFormat.AsInt64);
            var result7 = reader.Read("[12.12, 13.13, 14.14]", JSonReaderNumberFormat.AsDouble);
            var result8 = reader.Read("[12.12, 13.13, 14.14]", JSonReaderNumberFormat.AsDecimal);
            var result9 = reader.ReadAsJSonMutableObject("[12, 13, 14]", JSonReaderNumberFormat.AsInt32);
            var result10 = reader.ReadAsJSonMutableObject("[" + (uint.MaxValue + 1ul) + ", " + (uint.MaxValue + 2ul) + ", " + (uint.MaxValue + 3ul) + "]", JSonReaderNumberFormat.AsInt64);
            var result11 = reader.ReadAsJSonMutableObject("[12.12, 13.13, 14.14]", JSonReaderNumberFormat.AsDouble);
            var result12 = reader.ReadAsJSonMutableObject("[12.12, 13.13, 14.14]", JSonReaderNumberFormat.AsDecimal);

            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);
            Assert.IsNotNull(result4);
            Assert.IsNotNull(result5);
            Assert.IsNotNull(result6);
            Assert.IsNotNull(result7);
            Assert.IsNotNull(result8);
            Assert.IsNotNull(result9);
            Assert.IsNotNull(result10);
            Assert.IsNotNull(result11);
            Assert.IsNotNull(result12);
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseNumberWithInt32Forced_AndFail()
        {
            try
            {
                var reader = new JSonReader("[12.12, 13.13, 14.14]");
                reader.ReadAsJSonObject(JSonReaderNumberFormat.AsInt32);
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(1, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseNumberWithInt64Forced_AndFail()
        {
            try
            {
                var reader = new JSonReader("[" + ulong.MaxValue + "]");
                reader.ReadAsJSonObject(JSonReaderNumberFormat.AsInt64);
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(1, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidToken_AndFail()
        {
            try
            {
                var reader = new JSonReader("  \t /-~~");
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(4, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidTokenWithinArray_AndFail()
        {
            try
            {
                var reader = new JSonReader(" [ \t /// ]");
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(5, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidKeyword_AndFail()
        {
            try
            {
                var reader = new JSonReader("  \t abcdef");
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(4, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        [ExpectedException(typeof(JSonReaderException))]
        public void ParseInvalidKeywordWithinArray_AndFail()
        {
            try
            {
                var reader = new JSonReader(" [ \t abcdef ]");
                reader.ReadAsJSonObject();
            }
            catch (JSonReaderException ex)
            {
                Assert.AreEqual(0, ex.Line);
                Assert.AreEqual(5, ex.Offset);
                throw;
            }
        }

        [TestMethod]
        public void ParseNumberWithDecimalForced_AndCheckItemType()
        {
            var reader = new JSonReader((ulong.MaxValue + 1d).ToString(CultureInfo.InvariantCulture));
            var result = reader.ReadAsJSonObject(JSonReaderNumberFormat.AsDecimal);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.GetType().Name.Contains("DecimalDecimal"));
        }

        [TestMethod]
        public void ParseChineseTextWithEmbeddedArray()
        {
            var jsonText = LoadTestInputFile("chinese_encoding.json");

            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var internalData = reader.ReadAsJSonObject(result["DataObject"].StringValue);
            Assert.IsNotNull(internalData);
            Assert.AreEqual(2, internalData.Count);
            Assert.AreEqual("业务员", internalData[0]["EmployeeType"].StringValue);
        }

        [TestMethod]
        public void ParseAsIEnumerable()
        {
            var jsonText = @"[{""title"":""Title1"",""children"":[{""title"":""Child1"",""children"":[{""title"":""grandchild1"",""children"":[{""title"":""Huh""}]}] }] }, {""title"": ""Title2"" }]";

            JSonReader jr = new JSonReader(jsonText);
            IJSonObject json = jr.ReadAsJSonObject();
            IEnumerable items = json.ArrayItems;

            foreach (var arrayItem in json.ArrayItems)
            {
                //Console.WriteLine(arrayItem.ToString());
                foreach (var objItem in arrayItem.ObjectItems)
                {
                    Console.WriteLine(objItem);
                }
            }
        }
    }
}
