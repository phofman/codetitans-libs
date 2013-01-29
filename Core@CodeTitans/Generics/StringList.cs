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
using System.IO;
using System.Text;
using CodeTitans.Helpers;
using System.Collections;

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Class representing MacOS *.strings file.
    /// </summary>
    public class StringList : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _items;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StringList()
        {
            _items = new Dictionary<string, string>();
        }

        /// <summary>
        /// Private constructor to support reading from external sources.
        /// </summary>
        private StringList(Dictionary<string, string> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            _items = items;
        }

        #region Properties

        /// <summary>
        /// Gets the number of stored items.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets the value for element with given name.
        /// </summary>
        public string this[String key]
        {
            get { return _items[key]; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Checks if member element with given name exists.
        /// </summary>
        public bool Contains(String key)
        {
            return _items.ContainsKey(key);
        }

        /// <summary>
        /// Gets the value for element with given name.
        /// </summary>
        public bool TryGetValue(String key, out String value)
        {
            return _items.TryGetValue(key, out value);
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        public void Add(string key, string value)
        {
            if (_items.ContainsKey(key))
                _items[key] = value;
            else
                _items.Add(key, value);
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        public bool Remove(string key)
        {
            return _items.Remove(key);
        }

        /// <summary>
        /// Removes all items.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Writes all stored data to given output.
        /// </summary>
        public void Write(StringBuilder output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            foreach (var item in _items)
            {
                output.Append('"').Append(StringHelper.GetSecureString(item.Key)).Append("\" = \"");
                output.Append(StringHelper.GetSecureString(item.Value)).Append("\";\r\n");
            }
        }

        /// <summary>
        /// Writes all stored data to given output.
        /// </summary>
        public void Write(TextWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            foreach (var item in _items)
            {
                output.Write('"');
                output.Write(StringHelper.GetSecureString(item.Key));
                output.Write("\" = \"");
                output.Write(StringHelper.GetSecureString(item.Value));
                output.WriteLine("\";\r\n");
            }
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            Write(result);

            return result.ToString();
        }

        #endregion

        /// <summary>
        /// Reads strings file definition from given text.
        /// </summary>
        public static StringList Read(String text)
        {
            return Read(StringHelper.CreateReader(text));
        }

        /// <summary>
        /// Reads strings file definition from given text.
        /// </summary>
        public static StringList Read(TextReader reader)
        {
            return Read(StringHelper.CreateReader(reader));
        }

        private static StringList Read(IStringReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            // if there is nothing to read, return an empty instance:
            if (reader.IsEmpty)
                return new StringList();

            StringBuilder buffer = new StringBuilder();
            StringBuilder escapedUnicode = new StringBuilder();
            StringListToken token;
            Dictionary<string, string> result = new Dictionary<string, string>();
            string key = null;
            string value = null;
            bool identitySpot = false;

            do
            {
                token = ReadNextToken(reader);

                switch (token)
                {
                    case StringListToken.CommentLine:
                        // skip till the end of line:
                        StringHelper.ReadCommentChars(reader, false);
                        break;
                    case StringListToken.CommentMultiline:
                        StringHelper.ReadCommentChars(reader, true);
                        break;
                    case StringListToken.Semicolon:
                        // verify if semicolon is at the end of the 'stable' state:
                        if (!string.IsNullOrEmpty(key))
                        {
                            if (identitySpot && value != null)
                            {
                                if (result.ContainsKey(key))
                                    result[key] = value;
                                else
                                    result.Add(key, value);

                                // reset state:
                                key = null;
                                value = null;
                                identitySpot = false;
                            }
                            else
                            {
                                if (identitySpot)
                                    throw new FormatException("Incomplete definition at: (" + reader.Line + ":" + reader.LineOffset + ")");
                                throw new FormatException("Single text without valid assignment found: \"" + key + "\"");
                            }
                        }
                        break;
                    case StringListToken.Identity:
                        if (identitySpot)
                            throw new FormatException("Double identity operator found");
                        identitySpot = true;
                        if (string.IsNullOrEmpty(key))
                            throw new FormatException("Empty assignment found");
                        break;
                    case StringListToken.String:
                        {
                            if (key != null && value != null)
                                throw new FormatException("Invalid definition found at: (" + reader.Line + ":" + reader.LineOffset + ")");

                            StringHelperStatusCode status = StringHelper.ReadStringChars(reader, buffer, escapedUnicode, false);

                            // throw exceptions for selected errors:
                            switch (status)
                            {
                                case StringHelperStatusCode.UnexpectedEoF:
                                    throw new FormatException("Unexpected end of file while reading a string: \"" + buffer + "\"");
                                case StringHelperStatusCode.UnexpectedNewLine:
                                    throw new FormatException("Unexpected new line character in the middle of a string: \"" + buffer + "\"");
                                case StringHelperStatusCode.UnknownEscapedChar:
                                    throw new FormatException("Unknown escape combination: " + reader.CurrentChar);
                                case StringHelperStatusCode.TooShortEscapedChar:
                                    throw new FormatException("Invalid escape definition of Unicode character: " + reader.CurrentChar);
                                case StringHelperStatusCode.TooLongEscapedChar:
                                    throw new FormatException("Too long Unicode number definition: " + escapedUnicode);
                            }

                            if (identitySpot)
                            {
                                value = buffer.ToString();
                            }
                            else
                            {
                                if (key != null)
                                    throw new FormatException("Missing identity operator");

                                key = buffer.ToString();
                            }
                            buffer.Remove(0, buffer.Length);
                        }
                        break;
                    case StringListToken.Eof:

                        if (identitySpot)
                            throw new FormatException("Missing value for key at the end of the file");
                        if (!string.IsNullOrEmpty(key))
                            throw new FormatException("Missing definition for key at the of the file");

                        return new StringList(result);
                    case StringListToken.Invalid:
                        throw new FormatException("Invalid token found at (" + reader.Line + ":" + reader.LineOffset + ")");
                }
            }
            while (true);
        }

        private static StringListToken ReadNextToken(IStringReader reader)
        {
            // skip white spaces here:
            StringHelper.ReadWhiteChars(reader);

            if (reader.IsEof)
                return StringListToken.Eof;

            var currentChar = reader.CurrentChar;

            switch (currentChar)
            {
                case '/':
                    currentChar = reader.ReadNext();
                    if (currentChar == '/')
                        return StringListToken.CommentLine;
                    if (currentChar == '*')
                        return StringListToken.CommentMultiline;
                    throw new FormatException("Invalid comment token at (" + reader.Line + ":" + (reader.LineOffset - 1) + ")");
                case ';':
                    return StringListToken.Semicolon;
                case '"':
                    return StringListToken.String;
                case '=':
                    return StringListToken.Identity;
            }

            return StringListToken.Invalid;
        }

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the dictionary.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }

    internal enum StringListToken
    {
        Invalid,
        Eof,
        String,
        Semicolon,
        Identity,
        CommentLine,
        CommentMultiline
    }
}
