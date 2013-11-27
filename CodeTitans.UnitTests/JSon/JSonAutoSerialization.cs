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
    /// <summary>
    /// Summary description for JSonAutoSerialization
    /// </summary>
    [TestClass]
    public class JSonAutoSerialization
    {
        class JSonSampleClass : IJSonSerializable, IEquatable<JSonSampleClass>
        {
            public JSonSampleClass()
            {
                ID = Guid.NewGuid();
            }

            public JSonSampleClass(string name, int age, double salary)
            {
                ID = Guid.NewGuid();
                Name = name;
                Age = age;
                Salary = salary;
            }

            #region Properties

            public Guid ID
            { get; private set; }

            public string Name
            { get; private set; }

            public int Age
            { get; private set; }

            public double Salary
            { get; private set; }

            #endregion

            #region IJSonWritable Members

            public void Write(IJSonWriter output)
            {
                output.WriteObjectBegin();
                output.WriteMember("ID", ID);
                output.WriteMember("Name", Name);
                output.WriteMember("Age", Age);
                output.WriteMember("Salary", Salary);
                output.WriteObjectEnd();
            }

            #endregion

            #region IJSonReadable Members

            public void Read(IJSonObject input)
            {
                ID = input["ID"].GuidValue;
                Name = input["Name"].StringValue;
                Age = input["Age"].Int32Value;
                Salary = input["Salary"].DoubleValue;
            }

            #endregion

            public override bool Equals(object obj)
            {
                return base.Equals(obj as JSonSampleClass);
            }

            public override int GetHashCode()
            {
                return ID.GetHashCode();
            }

            #region IEquatable<JSonSampleClass> Members

            public bool Equals(JSonSampleClass other)
            {
                if (other != null)
                {
                    return other.ID == ID && other.Name == Name && other.Age == Age && other.Salary == Salary;
                }

                return false;
            }

            #endregion
        }

        private JSonReader reader;
        private JSonWriter writer;

        [TestInitialize]
        public void TestInit()
        {
            reader = new JSonReader();
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
        public void TestSerializeAndDeserialize()
        {
            JSonSampleClass o1 = new JSonSampleClass("Paweł", 30, 1001.1);
            JSonSampleClass d1 = new JSonSampleClass();

            Assert.AreNotEqual(o1, d1, "Can't be equal!");

            writer.Write(o1);
            var result = reader.ReadAsJSonObject(writer.ToString());

            d1.Read(result);

            Assert.IsTrue(o1.Equals(d1), "Object is not deserialized correctly!");
        }

        [TestMethod]
        public void TestSerializeArrayWithString()
        {
            string text = "Jakiś tekst!\r\n Paweł";

            writer.WriteArrayBegin();
            writer.WriteValue(text);
            writer.WriteArrayEnd();
            Assert.AreEqual(writer.ToString(), "[\r\n    \"Jakiś tekst!\\r\\n Paweł\"\r\n]", "Text is deserialized incorrectly!");

            var result = reader.Read(writer.ToString()) as object[];
            Assert.AreEqual(text, result[0], "Text after deserialization are not equal!");
        }

        [JSonSerializable(AllowAllFields=true, IgnoreStatic=false)]
        class SampleAttributedClass
        {
            [JSonMember("name", "Unknown")]
            private string _name;
            [JSonMember("age", 18)]
            private int _age;
            [JSonMember("address", "default", SkipWhenNull=true)]
            private string _address;

            [JSonMember("CurrentTime")]
            private DateTime _dt = DateTime.Now;

            private Guid testField;

            public const string Text = "Some Const Text, that is not supposed to be serialized!";
            public static bool StaticSampleField = false;

            public SampleAttributedClass()
            {
            }

            public SampleAttributedClass(string name, int age)
            {
                testField = Guid.NewGuid();
                _name = name;
                _age = age;
            }

            public string Name
            {
                get { return _name; }
            }

            public int Age
            {
                get { return _age; }
            }

            public string Address
            {
                get { return _address; }
                set { _address = value; }
            }
        }

        [JSonSerializable]
        class WrapperClass
        {
            static int _id = 0;

            public WrapperClass(object value, object data)
            {
                Value = value;
                Data = data;
                ++_id;
                ID = _id;
            }

            [JSonMember("id")]
            public int ID
            { get; private set; }

            [JSonMember("value")]
            public object Value
            { get; private set; }

            [JSonMember("data", SkipWhenNull=true)]
            public object Data
            { get; private set; }

        }

        [TestMethod]
        public void TestSerializeViaAttributes()
        {
            SampleAttributedClass o1 = new SampleAttributedClass("Paweł", 20);

            writer.Write(o1);
            var result = writer.ToString();

            Assert.IsNotNull(result, "Expected some serialized data");
            Assert.IsFalse(result.Contains(SampleAttributedClass.Text), "Unexpected 'const' field in serialized output");
            Assert.IsTrue(result.Contains("StaticSampleField"), "Expected static field in output");
        }

        [TestMethod]
        public void TestSerializeAndDeserializeViaSimpleAttributes()
        {
            SampleAttributedClass o1 = new SampleAttributedClass("Paweł", 20);

            writer.Write(o1);
            var result = writer.ToString();

            Assert.IsNotNull(result, "Expected some serialized data");
            Assert.IsFalse(result.Contains(SampleAttributedClass.Text), "Unexpected 'const' field in serialized output");
            Assert.IsTrue(result.Contains("StaticSampleField"), "Expected static field in output");

            SampleAttributedClass.StaticSampleField = true;
            var o2 = reader.ReadAsJSonObject(result).ToObjectValue<SampleAttributedClass>();

            Assert.IsNotNull(o2, "Correct value expected");
            Assert.AreEqual(o2.Name, o1.Name, "Name is not equal expected value");
            Assert.AreEqual(o2.Age, o1.Age, "Age is not equal expected value");
            Assert.AreEqual(o2.Address, "default", "Address should be equal to default value");

            // try to load array of elements:
            var o3 = reader.ReadAsJSonObject("[" + result + "]").ToObjectValue<IList<SampleAttributedClass>>();

            Assert.IsNotNull(o3, "Expected data to be read");

            o2 = o3[0];
            Assert.IsNotNull(o2, "Correct value expected");
            Assert.AreEqual(o2.Name, o1.Name, "Name is not equal expected value");
            Assert.AreEqual(o2.Age, o1.Age, "Age is not equal expected value");
            Assert.AreEqual(o2.Address, "default", "Address should be equal to default value");
        }

        [TestMethod]
        [ExpectedException(typeof(JSonMemberMissingException))]
        public void TestMissingMember()
        {
            var result = reader.ReadAsJSonObject("{ \"Name\": \"Jan\" }").ToObjectValue<SampleAttributedClass>();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void TestHierarchicalAutoSerialization()
        {
            var o1 = new WrapperClass(new WrapperClass(new string[] { "request", "response", "event" }, new WrapperClass(new WrapperClass(new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() }, null), null)), null);

            writer.Write(o1);
            var result = writer.ToString();

            Assert.IsNotNull(result, "Expected serialized object");
            Assert.IsFalse(result.Contains("null"), "Unexpected token found");
        }

        [JSonSerializable(AllowAllFields = true)]
        class ArrayContainingClass
        {
            [JSonMember("data1", null, SkipWhenNull = true)]
            private Array _data1;
            [JSonMember("data2", null, SkipWhenNull = true)]
            private Array _data2;

            public ArrayContainingClass()
            {
            }

            public ArrayContainingClass(Array keys, Array urls)
            {
                _data1 = keys;
                _data2 = urls;
            }

            public Array Data1
            {
                get { return _data1; }
            }

            public Array Data2
            {
                get { return _data2; }
            }
        }

        [TestMethod]
        public void TestReadWriteAbstractArrays()
        {
            JSonWriter writer = new JSonWriter();

            string[] data1 = { "123456", "78910" };
            string[] data2 = { "A", "B", "C" };

            var container = new ArrayContainingClass(data1, data2);
            writer.Write(container);

            Console.WriteLine("JSON:");
            Console.WriteLine(writer.ToString());

            var reader = new JSonReader();

            var readContainer = reader.ReadAsJSonObject(writer.ToString()).ToObjectValue<ArrayContainingClass>();
            Assert.IsNotNull(readContainer);
            Assert.IsNotNull(readContainer.Data1);
            Assert.IsNotNull(readContainer.Data2);
            Assert.AreEqual(2, readContainer.Data1.Length);
            Assert.AreEqual(3, readContainer.Data2.Length);
        }
    }
}
