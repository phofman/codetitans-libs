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
    /// <summary>
    /// Tests for INI files.
    /// </summary>
    [TestClass]
    public class IniFilesTests
    {
        [TestMethod]
        public void CreateEmptyIniStringsFile()
        {
            var iniFile = new IniStrings();

            Assert.IsNotNull(iniFile);
            Assert.IsNotNull(iniFile.Names);
            Assert.IsNotNull(iniFile.Sections);
            Assert.AreEqual(0, iniFile.Count);
        }

        [TestMethod]
        public void AddSectionIntoIniStringsFile()
        {
            var iniFile = new IniStrings();
            var section = new IniSection("Test");

            section.Comment = "a\r\nb\r\nc\r\n";
            section.Add(new IniSectionItem("Connection", " a, b, c, d", "definition of connection string"));
            section.Add(new IniSectionItem("Type", "selector"));

            iniFile.Add(section);

            Assert.AreEqual(1, iniFile.Count);
            Assert.IsTrue(iniFile.Contains("test"));
            Assert.IsTrue(iniFile["test"].Contains("connection"));
            Assert.IsNotNull(iniFile.ToString());
        }

        [TestMethod]
        public void ReadSimpleTextIniStrings()
        {
            var text = @"
# section comment
[Section1]
i1=a
# item comment
i2=' a '
[Section2]
x=1
y=2
z=3
";
            var iniFile = IniStrings.Read(text);

            VerifySimpleIniStrings(iniFile);
        }

        [TestMethod]
        public void ReadSimpleStreamIniStrings()
        {
            var text = @"
# section comment
[Section1]
i1=a
# item comment
i2=' a '
[Section2]
x=1
y=2
z=3
";
            var iniFile = IniStrings.Read(StringReaderTests.CreateStreamReader(text));

            VerifySimpleIniStrings(iniFile);
        }

        private static void VerifySimpleIniStrings(IniStrings iniFile)
        {
            Assert.IsNotNull(iniFile);
            Assert.AreEqual(2, iniFile.Count);

            var section1 = iniFile["section1"];
            Assert.IsNotNull(section1);
            Assert.AreEqual("section comment", section1.Comment);
            Assert.AreEqual(2, section1.Count);

            var i1 = section1["I1"];
            Assert.IsNotNull(i1);
            Assert.AreEqual("a", i1.Value);
            Assert.IsNull(i1.Comment);

            var i2 = section1["I2"];
            Assert.IsNotNull(i2);
            Assert.AreEqual(" a ", i2.Value);
            Assert.AreEqual("item comment", i2.Comment);

            var section2 = iniFile["secTion2"];
            Assert.IsNotNull(section2);
            Assert.IsNull(section2.Comment);
            Assert.AreEqual(3, section2.Count);

            var x = section2["x"];
            Assert.IsNotNull(x);
            Assert.AreEqual("1", x.Value);
            Assert.IsNull(x.Comment);
        }

        [TestMethod]
        public void ReadMoreCompliatedTextIniStrings()
        {
            var text = @"
;[connect name] will modify the connection if ADC.connect=""name""
;[connect default] will modify the connection if name is not found
;The Access is computed as follows:
;  (1) First take the access of the connect section.
;  (2) If a user entry is found, it will override.

[connect default]
;If we want to disable unknown connect values, we set Access to NoAccess
Access=NoAccess

[sql default]
;If we want to disable unknown sql values, we set Sql to an invalid query.
Sql="" ""

[connect CustomerDatabase]
Access=ReadWrite
Connect=""DSN=AdvWorks""

[sql CustomerById]
Sql=""SELECT * FROM Customers WHERE CustomerID = ?""

[connect AuthorDatabase]
Access=ReadOnly
Connect=""DSN=MyLibraryInfo;UID=MyUserID;PWD=MyPassword""

[userlist AuthorDatabase]
Administrator=ReadWrite

[sql AuthorById]
Sql=""SELECT * FROM Authors WHERE au_id = ?""

";
            var iniFile = IniStrings.Read(text);

            VerifyMoreComplicatedIniStrings(iniFile);
        }

        [TestMethod]
        public void ReadMoreCompliatedStreamIniStrings()
        {
            var text = @"
;[connect name] will modify the connection if ADC.connect=""name""
;[connect default] will modify the connection if name is not found
;The Access is computed as follows:
;  (1) First take the access of the connect section.
;  (2) If a user entry is found, it will override.

[connect default]
;If we want to disable unknown connect values, we set Access to NoAccess
Access=NoAccess

[sql default]
;If we want to disable unknown sql values, we set Sql to an invalid query.
Sql="" ""

[connect CustomerDatabase]
Access=ReadWrite
Connect=""DSN=AdvWorks""

[sql CustomerById]
Sql=""SELECT * FROM Customers WHERE CustomerID = ?""

[connect AuthorDatabase]
Access=ReadOnly
Connect=""DSN=MyLibraryInfo;UID=MyUserID;PWD=MyPassword""

[userlist AuthorDatabase]
Administrator=ReadWrite

[sql AuthorById]
Sql=""SELECT * FROM Authors WHERE au_id = ?""

";
            var iniFile = IniStrings.Read(StringReaderTests.CreateStreamReader(text));

            VerifyMoreComplicatedIniStrings(iniFile);
        }

        private static void VerifyMoreComplicatedIniStrings(IniStrings iniFile)
        {
            Assert.IsNotNull(iniFile);
            Assert.AreEqual(7, iniFile.Count);

            var section1 = iniFile["sql default"];
            Assert.IsNotNull(section1);

            var sql = section1["sql"];
            Assert.AreEqual(" ", sql.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReadInvalidIniStrings()
        {
            var text = @"; section with invalid item
[section1]
aaa
bbb
ccc
";
            var iniFile = IniStrings.Read(text);
        }
    }
}
