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
    /// <summary>
    /// All sample texts used comes from:
    /// http://developer.apple.com/library/mac/#documentation/Cocoa/Conceptual/LoadingResources/Strings/Strings.html
    /// </summary>
    [TestClass]
    public class StringFilesTests
    {
        [TestMethod]
        public void ReadSimpleText()
        {
            var text =
                @"
""Insert Element"" = ""Insert Element"";
""ErrorString_1"" = ""An unknown error occurred."";
";

            var reader = StringList.Read(text);

            Assert.IsNotNull(reader);
            Assert.AreEqual(reader.Count, 2);
            Assert.AreEqual(reader["ErrorString_1"], "An unknown error occurred.");
            Assert.IsFalse(reader.Contains("ErrorString_2"));
        }

        [TestMethod]
        public void ReadSimpleTextWithComments()
        {
            var text =
                @"
/* Insert Element menu item */
""Insert Element"" = ""Insert Element"";
/* Error string used for unknown error types. */
""ErrorString_1"" = ""An unknown error occurred."";
";

            var reader = StringList.Read(text);

            Assert.IsNotNull(reader);
            Assert.AreEqual(reader.Count, 2);
            Assert.AreEqual(reader["ErrorString_1"], "An unknown error occurred.");
            Assert.IsFalse(reader.Contains("ErrorString_2"));

            // and print to debug without comments:
            Debug.WriteLine(reader.ToString());
        }

        [TestMethod]
        public void ReadFrenchText()
        {
            var text = @"
/* A comment */
""Yes"" = ""Oui"";
""The same text in English"" = ""Le même texte en anglais"";
""No"" = ""No"";
";
            var reader = StringList.Read(text);

            Assert.IsNotNull(reader);
            Assert.AreEqual(reader.Count, 3);
            Assert.AreEqual(reader["Yes"], "Oui");
            Assert.AreEqual(reader["No"], "No");
            Assert.IsFalse(reader.Contains("YesNo"));
        }

        [TestMethod]
        public void ReadFormattableText()
        {
            var text = @"
/* Message in alert dialog when something fails */
""%@ Error! %@ failed!"" = ""%2$@ blah blah, %1$@ blah!"";

";
            var reader = StringList.Read(text);

            Assert.IsNotNull(reader);
            Assert.AreEqual(reader.Count, 1);
            Assert.AreEqual(reader["%@ Error! %@ failed!"], "%2$@ blah blah, %1$@ blah!");
            Assert.IsFalse(reader.Contains("Format"));
        }

        [TestMethod]
        public void ReadTextWithSpecialCharacters()
        {
            var text = @"
""File \""%@\"" cannot be opened"" = "" ... \r\n"";
""Type \""OK\"" when done"" = "" ... \t\r\n\f"";
";
            var reader = StringList.Read(text);

            Assert.IsNotNull(reader);
            Assert.AreEqual(reader.Count, 2);
            Assert.AreEqual(reader["File \"%@\" cannot be opened"], " ... \r\n");
            Assert.IsFalse(reader.Contains("Format"));

            // and print to debug without comments:
            Debug.WriteLine(reader.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryInvalidStatement_NoValue()
        {
            var text = @"
""File type"" = ;
";
            var reader = StringList.Read(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryInvalidStatement_NoIdentity()
        {
            var text = @"
""File type"" ""ABC"" ;
";
            var reader = StringList.Read(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryInvalidStatement_DoubleIdentity()
        {
            var text = @"
""File type"" == ""ABC"" ;
";
            var reader = StringList.Read(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryInvalidStatement_NoKey()
        {
            var text = @"
 == ""ABC"" ;
";
            var reader = StringList.Read(text);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TryInvalidStatement_NoSemicolon()
        {
            var text = @"
""Key"" = ""ABC""
";
            var reader = StringList.Read(text);
        }

        [TestMethod]
        public void TryInvalidStatement_DoubleSemicolon()
        {
            var text = @"
""Key"" = ""ABC"" ; ; ; ; ;
; ; ; ;;;;
""K3y"" = ""ABCD"" ;; ; ;
";
            var reader = StringList.Read(text);

            Assert.IsNotNull(reader);
            Assert.AreEqual(reader.Count, 2);
        }
    }
}
