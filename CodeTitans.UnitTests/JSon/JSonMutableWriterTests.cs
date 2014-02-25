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
    public class JSonMutableWriterTests
    {
        private JSonWriter writer;

        [TestInitialize]
        public void TestInit()
        {
            writer = new JSonWriter(true);
        }

        [TestCleanup]
        public void TestClean()
        {
            Console.WriteLine("Serialized JSON object:");
            Console.WriteLine(writer.ToString());
            writer.Close();
        }

        [TestMethod]
        public void WriteAnObjectWithSomeItems()
        {
            var o = JSonMutableObject.CreateDictionary();

            o.SetValue("Name", "Paweł");
            o.SetValue("age", 12);
            var currencies = o.SetArray("currencies");
            currencies.SetValue("PLN");
            currencies.SetValue("EUR");
            currencies.SetValue("USD");
            currencies.SetValue("XKG");

            writer.Write(o);

            Console.WriteLine(writer);
            Assert.AreNotEqual("{}", writer.ToString(), "Not expected an empty object here!");
        }

        [TestMethod]
        public void ReadMutableJSonAndUpdateIt()
        {
            const string jsonText = "[{\"minimumVersion\":\"1.0\",\"type\": null,\"channel\":\"\\/meta\\/handshake\",\"supportedConnectionTypes\":[\"request-response\"],\"successful\":true}]";

            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonMutableObject();

            Assert.IsNotNull(result, "Invalid data read!");
            Assert.AreEqual("1.0", result[0]["minimumVersion"].StringValue, "Expected other value for minimumVersion!");

            result[0].AsMutable.SetValue("minimumVersion", "2.0");
            Assert.AreEqual("2.0", result[0]["minimumVersion"].StringValue, "Expected updated value of minimumVersion!");

            result[0].AsMutable.Remove("minimumVersion");
            Assert.IsFalse(result[0].Contains("minimumVersion"), "Shoudn't contain the minimumVersion field!");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void TryToUpdateImmutableJSonObject()
        {
            var jsonText = "{ \"a\": 1, \"b\": 2 }";

            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonObject();

            Assert.IsNotNull(result, "Invalid data read!");
            Assert.AreEqual(1, result["a"].Int32Value, "Unexpected value for field 'a'.");

            result.AsMutable.SetValue("a", 3);
        }

        [TestMethod]
        public void ConvertImmutableJSonObjectToMutableAndUpdateIt()
        {
            var jsonText = "{ \"a\": 1, \"b\": 2, \"c\": [1,2,3,4] }";

            var reader = new JSonReader(jsonText);
            var result = reader.ReadAsJSonObject();

            Assert.IsNotNull(result, "Invalid data read!");
            Assert.AreEqual(1, result["a"].Int32Value, "Unexpected value for field 'a'.");

            var clone = result.CreateMutableClone();
            Assert.IsNotNull(clone, "Unexpected null clone!");
            Assert.AreEqual(1, clone["a"].Int32Value, "Unexpected value for clonned field 'a'.");
            Assert.IsTrue(clone["a"].IsMutable, "Value should be mutable!");

            clone["a"].AsMutable.SetValue(3);
            clone.SetValue("b", 4);
            clone["c"].AsMutable.SetValueAt(2, 55);
            Assert.AreEqual(3, clone["a"].Int32Value, "Expected value to be updated for field 'a'!");
            Assert.AreEqual(4, clone["b"].Int32Value, "Expected value to be updated for field 'b'!");
            Assert.AreEqual(55, clone["c"][2].Int32Value, "Expected array item to be updated!");

            // verify, that original object was left intact:
            Assert.AreEqual(1, result["a"].Int32Value, "Unexpected update!");
            Assert.AreEqual(2, result["b"].Int32Value, "Unexpected update!");
            Assert.AreEqual(3, result["c"][2].Int32Value, "Unexpected update!");
        }

        [TestMethod]
        public void CreateMutableObjectFromScratch()
        {
            var jo = JSonMutableObject.CreateDictionary();
            var now = DateTime.Now;
            var guid = Guid.NewGuid();

            jo.SetValue("name", "Paweł");
            jo.SetValue("age", 12);
            jo.SetValue("date", now, JSonDateTimeKind.Ticks);
            jo.SetValue("id", guid);
            jo.SetNull("school");

            var sa = jo.SetArray("items");
            sa.SetValue(1);
            sa.SetValue(2);
            sa.SetValue(3);

            var so = jo.SetDictionary("location");
            so.SetValue("latitude", 1.23d);
            so.SetValue("longigute", 1.24d);
            so.SetValue("city", "WRO");
            so.SetValue("country", "PL");

            var dates = JSonMutableObject.CreateArray();
            dates.SetValue(now.AddDays(1));
            dates.SetValue(now.AddMinutes(100));
            dates.SetValue(now.AddYears(2));

            jo.SetValue("dates", dates);

            writer.Write(jo);
            Assert.IsNotNull(writer.ToString(), "Should serialize as some kind of data!");

            var ic = jo.CreateImmutableClone(); // create immutable clone, where the values will be checked!

            Assert.IsTrue(ic.Contains("school"), "Should contain the 'school' item!");
            Assert.IsNull(ic["school"].StringValue, "'school' item should be equal to null!");
            Assert.AreEqual(now, ic["date"].ToDateTimeValue(JSonDateTimeKind.Ticks), "Incompatible current date!");
            Assert.AreEqual(3, ic["items"].Count, "There should be sub-array with 3 items!");
            Assert.AreEqual(2, ic["items"][1].Int32Value, "Invalid sub-array item value!");
            Assert.AreEqual(1.23d, ic["location"]["latitude"].DoubleValue, "Invalid latitude!");
        }
    }
}
