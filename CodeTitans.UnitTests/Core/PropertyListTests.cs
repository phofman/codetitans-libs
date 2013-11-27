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
using System.Diagnostics;
using CodeTitans.Core.Generics;
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
    public class PropertyListTests
    {
        [TestMethod]
        public void CreateEmpty()
        {
            var list = new PropertyList();
        }

        [TestMethod]
        public void AddRandomTypesElements()
        {
            var list = new PropertyList();
            var root = list.Root;

            Assert.IsNotNull(root);

            root.Add("Counter", 1);
            root.Add("Date", DateTime.Now);
            var array = root.AddNewArray("Achievements");

            Assert.IsNotNull(array);

            array.Add("A1");
            array.Add("A2");
            array.Add("A3");
            array.Add("A4");

            Assert.AreEqual(root.Count, 3, "First level should contain 3 elements");
            Assert.AreEqual(array.Length, 4, "Array should contain 4 elements");
            Assert.IsTrue(root.Contains("Achievements"));
            Assert.AreEqual(array[1].StringValue, "A2");

            StringBuilder result = new StringBuilder();
            list.Write(result);
            Debug.WriteLine(result.ToString());
        }

        [TestMethod]
        public void ParseFlatPropertyList()
        {
            var text = @"<?xml version=""1.0"" encoding=""UTF-8""?>
  <!DOCTYPE plist PUBLIC ""-//Apple Computer//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
  <plist version=""1.0"">
  <dict>
          <key>Name</key>
          <string>Pawel</string>

          <key>Surname</key>
          <string>Tester</string>

          <key>Street</key>
          <string>Somewhere</string>

          <key>State</key>
          <string>DL</string>

          <key>City</key>
          <string>Town</string>

          <key>Country</key>
          <string>PL</string>

          <key>ZipPostal</key>
          <string>12-345</string>

          <key>Age</key>
          <integer>123</integer>

        <key>Date</key>
        <date>2011-02-06T03:21:20</date>
  </dict>
  </plist>";

            PropertyList data = PropertyList.Read(text);

            Assert.IsNotNull(data);
            Assert.IsNotNull(data.Root);
            Assert.AreEqual(data.Root.Type, PropertyListItemTypes.Dictionary);
            Assert.AreEqual(data.Version, new Version(1, 0));

            Assert.AreEqual(data["Name"].StringValue, "Pawel");
            Assert.AreEqual(data["Age"].Int32Value, 123);
            Assert.AreEqual(data["Country"].StringValue, "PL");
            Assert.AreEqual(data["Date"].DateTimeValue, new DateTime(2011, 02, 06, 3, 21, 20));
        }

        [TestMethod]
        public void ParseAppleManualSample()
        {
            var text = @"<?xml version=""1.0"" encoding=""UTF-8""?>
           <!DOCTYPE plist PUBLIC ""-//Apple Computer//DTD PLIST 1.0//EN""
                   ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
           <plist version=""1.0"">
           <dict>
               <key>Year Of Birth</key>
               <integer>1965</integer>
               <key>Pets Names</key>
               <array/>
               <key>Picture</key>
               <data>
                   PEKBpYGlmYFCPA==
               </data>
               <key>City of Birth</key>
               <string>Springfield</string>
               <key>Name</key>
               <string>John Doe</string>
               <key>Kids Names</key>
               <array>
                   <string>John</string>
                   <string>Kyra</string>
               </array>
           </dict>
           </plist>
";

            PropertyList data = PropertyList.Read(text);

            Assert.IsNotNull(data);
            Assert.IsNotNull(data.Root);
            Assert.AreEqual(data.Root.Type, PropertyListItemTypes.Dictionary);
            Assert.AreEqual(data.Version, new Version(1, 0));

            var array = data.Root["Kids Names"];
            Assert.IsNotNull("array");
            Assert.AreEqual(array.Length, 2);
            Assert.AreEqual(array[1].StringValue, "Kyra");
        }

        [TestMethod]
        public void ParseArrayInsideDictionary()
        {
            var text = @"<?xml version=""1.0"" encoding=""UTF-8""?>
           <!DOCTYPE plist PUBLIC ""-//Apple Computer//DTD PLIST 1.0//EN""
                   ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
           <plist version=""1.0"">
           <dict>
               <key>Kids</key>
               <array>
                   <string>ABC</string>
                   <string>EFG</string>
               </array>
           </dict>
           </plist>
";

            PropertyList data = PropertyList.Read(text);

            Assert.IsNotNull(data);
            Assert.IsNotNull(data.Root);
            Assert.AreEqual(data.Root.Type, PropertyListItemTypes.Dictionary);
            Assert.AreEqual(data.Version, new Version(1, 0));

            var array = data["Kids"];
            Assert.IsNotNull("array");
            Assert.AreEqual(array.Length, 2);
            Assert.AreEqual(array[1].StringValue, "EFG");
        }

        [TestMethod]
        public void ParseDictionaryInsideArray()
        {
            var text = @"<?xml version=""1.0"" encoding=""UTF-8""?>
           <!DOCTYPE plist PUBLIC ""-//Apple Computer//DTD PLIST 1.0//EN""
                   ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
           <plist version=""1.0"">
           <dict>
               <key>Kids</key>
               <array>
                   <dict>
                        <key>Name</key>
                        <string>Ala</string>
                        <key>Age</key>
                        <integer>1</integer>
                   </dict>
               </array>
           </dict>
           </plist>
";

            PropertyList data = PropertyList.Read(text);

            Assert.IsNotNull(data);
            Assert.IsNotNull(data.Root);
            Assert.AreEqual(data.Root.Type, PropertyListItemTypes.Dictionary);
            Assert.AreEqual(data.Version, new Version(1, 0));

            var array = data["Kids"];
            Assert.IsNotNull("array");
            Assert.AreEqual(array.Length, 1);

            var dict = array[0];

            Assert.IsNotNull(dict);
            Assert.AreEqual(dict["Name"].StringValue, "Ala");
            Assert.AreEqual(dict["Age"].Int32Value, 1);
        }

        [TestMethod]
        public void OverwriteItemsWithTheSameName()
        {
            var data = new PropertyList();

            data.Add("A", 100.1);
            data.Add("B", 1);
            data.Add("A", true);

            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(true, data["A"].BooleanValue);
            Assert.AreNotEqual(100.1, data["A"].DoubleValue);
        }

        [TestMethod]
        public void RemoveItemByKeyFromDictionary()
        {
            var data = new PropertyList();

            data.Add("A", 1);
            Assert.AreEqual(1, data.Count);

            data.Remove("A");
            Assert.AreEqual(0, data.Count);

            Assert.AreEqual(-1, data["A", -1].Int32Value);
        }

        [TestMethod]
        public void RemoveItemByIndexFromDictionary()
        {
            var data = new PropertyList();

            data.Add("XXX", "here!");
            Assert.AreEqual("here!", data["XXX"].StringValue);
            Assert.AreEqual(1, data.Count);

            data.RemoveAt(0);
            Assert.AreEqual(0, data.Count);
            Assert.IsFalse(data.Contains("XXX"));
        }

        [TestMethod]
        public void RemoveItemByKeyFromArray()
        {
            var data = new PropertyList();

            var array = data.AddNewArray("Array");
            Assert.IsNotNull(array);

            array.Add("item1");
            array.Add("item2");
            array.Add(1);
            array.Add(2);
            Assert.AreEqual(4, array.Count);

            var item = array.Remove("xyz");
            Assert.IsNull(item);
        }

        [TestMethod]
        public void ClearItems()
        {
            var data = new PropertyList();

            data.Add("A", 1);
            data.Add("B", 2);
            data.Add("C", 3);
            data.Add("D", 4);
            data.Add("E", 5);
            data.Add("F", 6);
            Assert.AreEqual(6, data.Count);

            data.Clear();
            Assert.AreEqual(0, data.Count);
            Assert.IsFalse(data.Contains("A"));
            Assert.IsFalse(data.Contains("B"));
            Assert.IsFalse(data.Contains("C"));
            Assert.IsFalse(data.Contains("D"));
            Assert.IsFalse(data.Contains("E"));
            Assert.IsFalse(data.Contains("F"));
        }

        [TestMethod]
        public void LoadMultilevelItems()
        {
            var input = @"<?xml version=""1.0"" encoding=""UTF-8""?> 
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd""> 
<plist version=""1.0""> 
  <dict> 
   <key>section0</key> 
     <dict> 
       <key>key0</key> 
         <dict> 
           <key>name</key> 
           <string>Title</string> 
           <key>type</key> 
           <string>text</string> 
           <key>filter</key> 
           <false/> 
         </dict> 
       <key>key1</key> 
         <dict> 
           <key>name</key> 
           <string>Season</string> 
           <key>type</key> 
           <string>text</string>             
           <key>filter</key> 
           <false/>          
         </dict> 
     </dict> 
  </dict>
</plist>";

            var data = PropertyList.Read(input);

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Contains("section0"));

            var section0 = data["section0"];
            Assert.IsNotNull(section0);
            Assert.IsTrue(section0.Contains("key0"));

            var key0 = section0["key0"];
            Assert.IsNotNull(key0);

            Assert.AreEqual("Title", key0["name"].StringValue);
            Assert.AreEqual("text", key0["type"].StringValue);
            Assert.IsFalse(key0["filter"].BooleanValue);
            key0.Add("filter", true);
        }

        [TestMethod]
        public void AddBinaryData()
        {
            var propertyList = new PropertyList();
            byte[] data = new byte[] { 20, 25, 35, 45, 60, 1, 0, 99 };

            propertyList.Add("new_element", data);
            var text = propertyList.ToString();

            Assert.IsNotNull(text);

            var loadedList = PropertyList.Read(text);
            var loadedData = loadedList["new_element"].BinaryValue;

            Assert.AreEqual(loadedData.Length, data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(data[i], loadedData[i]);
            }
        }
    }
}
