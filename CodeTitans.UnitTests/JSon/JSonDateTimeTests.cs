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
    public class JSonDateTimeTests
    {
        [TestMethod]
        public void ParseISODateWithTimeZone()
        {
            var reader = new JSonReader("\"2012-06-16T21:09:45+00:00\"");
            var item = reader.ReadAsJSonObject();
            var date = item.DateTimeValue;

            Assert.AreEqual(new DateTime(2012, 06, 16, 21, 9, 45, DateTimeKind.Utc).ToLocalTime(), date);
        }

        [TestMethod]
        public void ParseISODateWithoutTimeZone()
        {
            var reader = new JSonReader("\"2012-05-17 09:58:33\"");
            var item = reader.ReadAsJSonObject();
            var date = item.DateTimeValue;

            Assert.AreEqual(new DateTime(2012, 05, 17, 09, 58, 33, DateTimeKind.Local), date);
        }

        [TestMethod]
        public void ParseDateWithoutTimeAsUnspecified()
        {
            var reader = new JSonReader("\"1999-01-07\"");
            var item = reader.ReadAsJSonObject();
            var date = item.DateTimeValue;

            Assert.AreEqual(new DateTime(1999, 01, 07, 0, 0, 0, DateTimeKind.Unspecified), date);
        }

        [TestMethod]
        public void ParseSerializedCurrentDateAsISO()
        {
            var writer = new JSonWriter();
            var now = DateTime.Now;

            // remove miliseconds:
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Kind);

            using(writer.WriteObject())
                writer.WriteMember("date", now);
            
            var reader = new JSonReader(writer.ToString());
            var item = reader.ReadAsJSonObject();
            var date = item["date"].DateTimeValue;

            Assert.AreEqual(now, date);
        }

        [TestMethod]
        public void ParseSerializedCurrentDateAsJavaScript()
        {
            var writer = new JSonWriter();
            var now = DateTime.Now;

            // remove miliseconds:
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Kind);

            using (writer.WriteObject())
                writer.WriteMember("date", now, JSonWriterDateFormat.JavaScript);

            var reader = new JSonReader(writer.ToString());
            var item = reader.ReadAsJSonObject();
            var date = item["date"].DateTimeValue;

            Assert.AreEqual(now, date);
        }

        [TestMethod]
        public void ParseSerializedCurrentDateAsEpochMilliseconds()
        {
            var writer = new JSonWriter();
            var now = DateTime.Now;

            using (writer.WriteObject())
                writer.WriteMember("date", now, JSonWriterDateFormat.UnixEpochMilliseconds);

            // remove below milliseconds:
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond, now.Kind);

            var reader = new JSonReader(writer.ToString());
            var item = reader.ReadAsJSonObject();
            var date = item["date"].DateTimeValue;

            Assert.AreNotEqual(0, item["date"].Int64Value);
            Assert.AreEqual(now, date);
        }

        [TestMethod]
        public void ParseSerializedCurrentDateAsEpochSeconds()
        {
            var writer = new JSonWriter();
            var now = DateTime.Now;

            using (writer.WriteObject())
                writer.WriteMember("date", now, JSonWriterDateFormat.UnixEpochSeconds);

            // remove below seconds:
            now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, 0, now.Kind);

            var reader = new JSonReader(writer.ToString());
            var item = reader.ReadAsJSonObject();
            var date = item["date"].ToDateTimeValue(JSonDateTimeKind.UnixEpochSeconds);

            Assert.AreNotEqual(0, item["date"].Int64Value);
            Assert.AreEqual(now, date);
        }


        [TestMethod]
        public void ParseCurrentDate()
        {
            var reader = new JSonReader("\"/DATE()/\"");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var currentDate = DateTime.Now;
            var date = result.DateTimeValue;

            Assert.AreEqual(currentDate.Year, date.Year);
            Assert.AreEqual(currentDate.Month, date.Month);
            Assert.AreEqual(currentDate.Day, date.Day);
        }

        [TestMethod]
        public void ParseDate1970_Variant1()
        {
            var reader = new JSonReader("\"/DATE(0)/\"");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var date = result.DateTimeValue.ToUniversalTime();

            Assert.AreEqual(1970, date.Year);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Day);
        }

        [TestMethod]
        public void ParseDate1970_Variant2()
        {
            var reader = new JSonReader("\"/ DATE (" + (2 * 60 * 60 * 1000) + " )/\"");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var date = result.DateTimeValue.ToUniversalTime();

            Assert.AreEqual(1970, date.Year);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual(2, date.Hour);
        }

        [TestMethod]
        public void ParseDate1970_Variant3()
        {
            var reader = new JSonReader("\"\\/ DATE (" + (11 * 60 * 60 * 1000 + 22 * 60 * 1000 + 30 * 1000) + " )\\/\"");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var date = result.DateTimeValue.ToUniversalTime();

            Assert.AreEqual(1970, date.Year);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual(11, date.Hour);
            Assert.AreEqual(22, date.Minute);
            Assert.AreEqual(30, date.Second);
        }

        [TestMethod]
        public void ParseDate1970_Variant4()
        {
            var reader = new JSonReader("\"@" + (11 * 60 * 60 * 1000 + 22 * 60 * 1000 + 30 * 1000) + " @\" ");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var date = result.DateTimeValue.ToUniversalTime();

            Assert.AreEqual(1970, date.Year);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual(11, date.Hour);
            Assert.AreEqual(22, date.Minute);
            Assert.AreEqual(30, date.Second);
        }

        [TestMethod]
        public void ParseExplicitDate_Varian1()
        {
            var reader = new JSonReader("\"\\/ DATE (1920,12,31)\\/\"");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var date = result.DateTimeValue.ToUniversalTime();

            Assert.AreEqual(1920, date.Year);
            Assert.AreEqual(12, date.Month);
            Assert.AreEqual(31, date.Day);
        }

        [TestMethod]
        public void ParseExplicitDate_Varian2()
        {
            var reader = new JSonReader("\"\\/ DATE (1999,  1 , 1, 12, 22, 33, 500)\\/\"");
            var result = reader.ReadAsJSonObject();
            Assert.IsNotNull(result);

            var date = result.DateTimeValue.ToUniversalTime();
            Assert.AreEqual(1999, date.Year);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual(12, date.Hour);
            Assert.AreEqual(22, date.Minute);
            Assert.AreEqual(33, date.Second);
            Assert.AreEqual(500, date.Millisecond);
        }
    }
}
