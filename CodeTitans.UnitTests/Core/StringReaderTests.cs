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
    /// <summary>
    /// Extra tests for reading strings.
    /// </summary>
    [TestClass]
    public class StringReaderTests
    {
        [TestMethod]
        public void CreateStreamReader()
        {
            var reader = StringHelper.CreateReader(CreateStreamReader("abc"));

            VerifyInitialState(reader);
        }

        [TestMethod]
        public void CreateTextReader()
        {
            var reader = StringHelper.CreateReader("abc\r\ndef\r\n");
            VerifyInitialState(reader);
        }

        public static TextReader CreateStreamReader(string initialText)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(initialText ?? string.Empty));
            return new StreamReader(stream, Encoding.UTF8);
        }

        private static void VerifyInitialState(IStringReader reader)
        {
            Assert.IsNotNull(reader);
            Assert.AreEqual(char.MinValue, reader.CurrentChar);
            Assert.IsFalse(reader.IsEof);
            Assert.IsFalse(reader.IsEmpty);
            Assert.AreEqual(0, reader.Line);
            Assert.AreEqual(-1, reader.LineOffset);
        }

        [TestMethod]
        public void ReadWholeTextAndVerifyStates()
        {
            var reader = StringHelper.CreateReader("a\r\nb\r\n");

            VerifyReadingSimpleText(reader);
        }

        [TestMethod]
        public void ReadWholeStreamAndVerifyStates()
        {
            var reader = StringHelper.CreateReader(CreateStreamReader("a\r\nb\r\n"));

            VerifyReadingSimpleText(reader);
        }

        private static void VerifyReadingSimpleText(IStringReader reader)
        {
            Assert.IsNotNull(reader);

            // read first line:
            var c = reader.ReadNext();
            Assert.AreEqual('a', c);
            Assert.AreEqual('a', reader.CurrentChar);
            Assert.AreEqual(0, reader.Line);
            Assert.AreEqual(0, reader.LineOffset);
            Assert.IsFalse(reader.IsEmpty);

            c = reader.ReadNext();
            Assert.AreEqual('\r', c);
            Assert.AreEqual(0, reader.Line);
            Assert.AreEqual(1, reader.LineOffset);
            Assert.IsFalse(reader.IsEmpty);

            c = reader.ReadNext();
            Assert.AreEqual('\n', c);
            Assert.AreEqual('\n', reader.CurrentChar);
            Assert.AreEqual(0, reader.Line);
            Assert.AreEqual(2, reader.LineOffset);
            Assert.IsFalse(reader.IsEmpty);

            c = reader.ReadNext();
            Assert.AreEqual('b', c);
            Assert.AreEqual('b', reader.CurrentChar);
            Assert.AreEqual(1, reader.Line);
            Assert.AreEqual(0, reader.LineOffset);
            Assert.IsFalse(reader.IsEmpty);

            c = reader.ReadNext();
            Assert.AreEqual('\r', c);
            Assert.AreEqual('\r', reader.CurrentChar);
            Assert.AreEqual(1, reader.Line);
            Assert.AreEqual(1, reader.LineOffset);
            Assert.IsFalse(reader.IsEmpty);

            c = reader.ReadNext();
            Assert.AreEqual('\n', c);
            Assert.AreEqual('\n', reader.CurrentChar);
            Assert.AreEqual(1, reader.Line);
            Assert.AreEqual(2, reader.LineOffset);
            Assert.IsFalse(reader.IsEof);
            Assert.IsFalse(reader.IsEmpty);

            c = reader.ReadNext();
            Assert.AreEqual('\0', c);
            Assert.AreEqual('\0', reader.CurrentChar);
            Assert.AreEqual(2, reader.Line);
            Assert.AreEqual(-1, reader.LineOffset);
            Assert.IsTrue(reader.IsEof);
            Assert.IsFalse(reader.IsEmpty);
        }

        [TestMethod]
        public void ReadEmptyTextByLines()
        {
            var reader = StringHelper.CreateReader((string) null);

            VerifyEmptyReadingByLines(reader);
        }

        [TestMethod]
        public void ReadEmptyStreamByLines()
        {
            var reader = StringHelper.CreateReader(CreateStreamReader(null));

            VerifyEmptyReadingByLines(reader);
        }

        private static void VerifyEmptyReadingByLines(IStringReader reader)
        {
            Assert.IsNotNull(reader);
            Assert.IsTrue(reader.IsEmpty);
            Assert.AreEqual(null, reader.ReadLine());
            Assert.AreEqual(0, reader.Line);
            Assert.AreEqual(-1, reader.LineOffset);
            Assert.AreEqual(char.MinValue, reader.CurrentChar);
        }

        [TestMethod]
        public void ReadTextByLines()
        {
            var reader = StringHelper.CreateReader("\n\r\n");

            VerifyReadingByLines(reader);
        }

        [TestMethod]
        public void ReadStreamByLines()
        {
            var reader = StringHelper.CreateReader(CreateStreamReader("\n\r\n"));

            VerifyReadingByLines(reader);
        }

        private static void VerifyReadingByLines(IStringReader reader)
        {
            var line = reader.ReadLine();
            Assert.IsNotNull(reader);
            Assert.AreEqual(string.Empty, line);
            Assert.AreEqual('\n', reader.CurrentChar);
            Assert.AreEqual(0, reader.Line);
            Assert.AreEqual(0, reader.LineOffset);

            line = reader.ReadLine();
            Assert.AreEqual(string.Empty, line);
            Assert.AreEqual('\n', reader.CurrentChar);
            Assert.AreEqual(1, reader.Line);
            Assert.AreEqual(1, reader.LineOffset);
        }
    }
}
