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
using System.Collections.Generic;
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
    /// Test class for marker parsing.
    /// </summary>
    [TestClass]
    public class MarkerStringsTests
    {
        private List<string> texts;
        private List<string> tags;
        private List<string> invocations;

        private int Parse(string text, string startTag, string endTag)
        {
            texts = new List<string>();
            tags = new List<string>();
            invocations = new List<string>();

            return MarkerStrings.Parse(text, this, startTag, endTag, OnText, OnTagContent);
        }

        private void OnTagContent(object o, string text)
        {
            Assert.IsNotNull(text);

            tags.Add(text);
            invocations.Add("m:" + text);
        }

        private void OnText(object o, string text)
        {
            Assert.IsNotNull(text);

            texts.Add(text);
            invocations.Add("t:" + text);
        }

        [TestMethod]
        public void TextSingleLine()
        {
            var result = Parse("abcdefghijkl", "%", "%");

            Assert.AreEqual(0, result);
            Assert.AreEqual(0, tags.Count);
            Assert.AreEqual(1, texts.Count);
            Assert.AreEqual("abcdefghijkl", texts[0]);
            Assert.AreEqual("t:abcdefghijkl", invocations[0]);
        }

        [TestMethod]
        public void TextMultiLine()
        {
            var result = Parse("abc\ndef", "%", "%");

            Assert.AreEqual(0, result);
            Assert.AreEqual(0, tags.Count);
            Assert.AreEqual(1, texts.Count);
            Assert.AreEqual("abc\r\ndef", texts[0]);
            Assert.AreEqual("t:abc\r\ndef", invocations[0]);
        }

        [TestMethod]
        public void TextSingleLineWithTag()
        {
            var result = Parse("abc%aaa%", "%", "%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(1, texts.Count);
            Assert.AreEqual("abc", texts[0]);
            Assert.AreEqual("aaa", tags[0]);
            Assert.AreEqual("t:abc", invocations[0]);
            Assert.AreEqual("m:aaa", invocations[1]);
        }

        [TestMethod]
        public void TextSingleLineWithEmptyTag()
        {
            var result = Parse("abc%%", "%", "%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(1, texts.Count);
            Assert.AreEqual("abc", texts[0]);
            Assert.AreEqual("", tags[0]);
            Assert.AreEqual("t:abc", invocations[0]);
            Assert.AreEqual("m:", invocations[1]);
        }

        [TestMethod]
        public void TextMultiLineWithTag()
        {
            var result = Parse("abc\r\ndef\r\n%aaa%xyz", "%", "%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(2, texts.Count);
            Assert.AreEqual("abc\r\ndef\r\n", texts[0]);
            Assert.AreEqual("xyz", texts[1]);
            Assert.AreEqual("aaa", tags[0]);
            Assert.AreEqual("t:abc\r\ndef\r\n", invocations[0]);
            Assert.AreEqual("m:aaa", invocations[1]);
            Assert.AreEqual("t:xyz", invocations[2]);
        }

        [TestMethod]
        public void EmptyTagOnly()
        {
            var result = Parse("%%", "%", "%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(0, texts.Count);
            Assert.AreEqual("", tags[0]);
            Assert.AreEqual("m:", invocations[0]);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void InvalidTagOnly()
        {
            Parse("%", "%", "%");

            Assert.Fail("Shoult throw exception");
        }

        [TestMethod]
        public void TwoTagsInARowOnly()
        {
            var result = Parse("%ab%%cde%", "%", "%");

            Assert.AreEqual(2, result);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual(0, texts.Count);
            Assert.AreEqual("ab", tags[0]);
            Assert.AreEqual("cde", tags[1]);
            Assert.AreEqual("m:ab", invocations[0]);
            Assert.AreEqual("m:cde", invocations[1]);
        }

        [TestMethod]
        public void TwoTagsInTwoLines()
        {
            var result = Parse("%ab%\n%cde%", "%", "%");

            Assert.AreEqual(2, result);
            Assert.AreEqual(2, tags.Count);
            Assert.AreEqual(1, texts.Count);
            Assert.AreEqual("\r\n", texts[0]);
            Assert.AreEqual("ab", tags[0]);
            Assert.AreEqual("cde", tags[1]);
            Assert.AreEqual("m:ab", invocations[0]);
            Assert.AreEqual("t:\r\n", invocations[1]);
            Assert.AreEqual("m:cde", invocations[2]);
        }

        [TestMethod]
        public void MultiLineTag()
        {
            var result = Parse("%ab\ncd\nef%", "%", "%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(0, texts.Count);
            Assert.AreEqual("ab\r\ncd\r\nef", tags[0]);
            Assert.AreEqual("m:ab\r\ncd\r\nef", invocations[0]);
        }

        [TestMethod]
        public void MultiCharTag()
        {
            var result = Parse("abc%=def=%ghi", "%=", "=%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(2, texts.Count);
            Assert.AreEqual("def", tags[0]);
            Assert.AreEqual("abc", texts[0]);
            Assert.AreEqual("ghi", texts[1]);
            Assert.AreEqual("t:abc", invocations[0]);
            Assert.AreEqual("m:def", invocations[1]);
            Assert.AreEqual("t:ghi", invocations[2]);
        }

        [TestMethod]
        public void MultiCharMultiLineTag()
        {
            var result = Parse("abc%=\ndef\n=%ghi", "%=", "=%");

            Assert.AreEqual(1, result);
            Assert.AreEqual(1, tags.Count);
            Assert.AreEqual(2, texts.Count);
            Assert.AreEqual("\r\ndef\r\n", tags[0]);
            Assert.AreEqual("abc", texts[0]);
            Assert.AreEqual("ghi", texts[1]);
            Assert.AreEqual("t:abc", invocations[0]);
            Assert.AreEqual("m:\r\ndef\r\n", invocations[1]);
            Assert.AreEqual("t:ghi", invocations[2]);
        }

        [TestMethod]
        public void RealExample()
        {
            var result = Parse("Hello %name%,\r\nGood news for you: %\nnews\n%.\nAre you\nINTERESTED in?\nJust reply %OK% to this message.", "%", "%");

            Assert.AreEqual(3, result);
            Assert.AreEqual(3, tags.Count);
            Assert.AreEqual(4, texts.Count);
            Assert.AreEqual("name", tags[0]);
            Assert.AreEqual("\r\nnews\r\n", tags[1]);
            Assert.AreEqual("OK", tags[2]);
            Assert.AreEqual("t:Hello ", invocations[0]);
            Assert.AreEqual("m:name", invocations[1]);
        }
    }
}
