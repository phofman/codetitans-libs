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
using System.IO;
using System.Text;

namespace CodeTitans.Helpers
{
    /// <summary>
    /// Helper class for managing string operations.
    /// </summary>
#if DEBUG && CODETITANS_LIB_CORE
    public
#else
    internal
#endif
    static class StringHelper
    {
        /// <summary>
        /// Digits being a part of hexadecimal number.
        /// </summary>
        private const string HexDigits = "0123456789abcdefABCDEF";

        /// <summary>
        /// Digits that represent the unsafe characters inside string and using them should actually be replaced by combinations.
        /// </summary>
        private static readonly char[] UnsecureChars = new char[] { '"', '\\', '\b', '\t', '\r', '\n', '\f' };

        #region Creation

        /// <summary>
        /// Creates a reader instance for a given string.
        /// </summary>
        public static IStringReader CreateReader(string text)
        {
            return new StringReaderWrapper(text);
        }

        /// <summary>
        /// Creates a reader instance for a given text reader.
        /// </summary>
        public static IStringReader CreateReader(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new TextReaderWrapper(reader);
        }

        #endregion

        #region Writing

        /// <summary>
        /// Gets the secured string, while special characters are presented as escaped ones.
        /// It is used by JSON writer and StringFiles writer mostly.
        /// </summary>
        internal static string GetSecureString(string value)
        {
            // if there are no 'unsafe' escape chars, then return the same instance,
            // otherwise rewrite the whole string:
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.IndexOfAny(UnsecureChars) < 0)
                return value;

            /////////////////////////////////////////////////////////////////////

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                switch (c)
                {
                    case '\\':
                        result.Append("\\\\");
                        break;
                    case '"':
                        result.Append("\\\"");
                        break;
                    case '\b':
                        result.Append("\\b");
                        break;
                    case '\t':
                        result.Append("\\t");
                        break;
                    case '\r':
                        result.Append("\\r");
                        break;
                    case '\n':
                        result.Append("\\n");
                        break;
                    case '\f':
                        result.Append("\\f");
                        break;
                    default:
                        result.Append(c);

                        /*
                        int ansiChar = (int)c;
                        if (ansiChar >= 32 && ansiChar <= 128)
                            result.Append(c);
                        else
                            result.Append("\\u").Append(Convert.ToString(ansiChar, 16).PadLeft(4, '0'));
                         */
                        break;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Writes the secured string, while special characters are presented as escaped ones.
        /// It is used by JSON writer and StringFiles writer mostly.
        /// </summary>
        internal static void WriteSecureString(TextWriter output, string value)
        {
            // if there are no 'unsafe' escape chars, then just write it:
            if (string.IsNullOrEmpty(value))
                return;

            if (value.IndexOfAny(UnsecureChars) < 0)
            {
                output.Write(value);
                return;
            }

            /////////////////////////////////////////////////////////////////////

            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];

                switch (c)
                {
                    case '\\':
                        output.Write("\\\\");
                        break;
                    case '"':
                        output.Write("\\\"");
                        break;
                    case '\b':
                        output.Write("\\b");
                        break;
                    case '\t':
                        output.Write("\\t");
                        break;
                    case '\r':
                        output.Write("\\r");
                        break;
                    case '\n':
                        output.Write("\\n");
                        break;
                    case '\f':
                        output.Write("\\f");
                        break;
                    default:
                        output.Write(c);

                        /*
                        int ansiChar = (int)c;
                        if (ansiChar >= 32 && ansiChar <= 128)
                            output.Write(c);
                        else
                            output.Write("\\u");
                            output.Write(Convert.ToString(ansiChar, 16).PadLeft(4, '0'));
                         */
                        break;
                }
            }
        }

        #endregion

        #region Reading

        /// <summary>
        /// Reads from the current input stream all the whitespaces.
        /// </summary>
        internal static StringHelperStatusCode ReadWhiteChars(IStringReader reader)
        {
            int count;
            return ReadWhiteChars(reader, out count);
        }

        /// <summary>
        /// Reads from the current input stream all the whitespaces.
        /// </summary>
        internal static StringHelperStatusCode ReadWhiteChars(IStringReader reader, out int count)
        {
            count = 0;

            do
            {
                char currentChar = reader.ReadNext();

                if (reader.IsEof)
                    return StringHelperStatusCode.UnexpectedEoF;

                if (!char.IsWhiteSpace(currentChar))
                    break;

                count++;
            }
            while (true);

            return StringHelperStatusCode.Success;
        }

        /// <summary>
        /// Reads characters that might be a number and copies them to given output.
        /// </summary>
        internal static StringHelperStatusCode ReadDecimalNumberChars(IStringReader reader, StringBuilder output)
        {
            do
            {
                var currentChar = reader.ReadNext();

                if (char.IsDigit(currentChar) || currentChar == '-' || currentChar == '+' || currentChar == '.'
                        || currentChar == 'e' || currentChar == 'E')
                {
                    output.Append(currentChar);
                }
                else
                    break;
            }
            while (true);

            return StringHelperStatusCode.Success;
        }

        /// <summary>
        /// Reads characters that might be a number and copies them to given output.
        /// </summary>
        internal static StringHelperStatusCode ReadIntegerNumberChars(IStringReader reader, StringBuilder output)
        {
            do
            {
                var currentChar = reader.ReadNext();

                if (char.IsDigit(currentChar) || currentChar == '-' || currentChar == '+')
                {
                    output.Append(currentChar);
                }
                else
                    break;
            }
            while (true);

            return StringHelperStatusCode.Success;
        }

        private static StringHelperStatusCode AddUnicodeChar(StringBuilder output, StringBuilder number, bool clearNumber)
        {
            if (number.Length < 4)
                return StringHelperStatusCode.TooShortEscapedChar;

            // Unicode number might have only 4 digits:
            if (number.Length > 4)
                return StringHelperStatusCode.TooLongEscapedChar;

            int charNumber;

            if (!NumericHelper.TryParseHexInt32(number.ToString(), out charNumber))
                return StringHelperStatusCode.UnknownEscapedChar;

            output.Append(Convert.ToChar(charNumber));
            if (clearNumber)
                number.Remove(0, number.Length);

            return StringHelperStatusCode.Success;
        }

        private static StringHelperStatusCode ReadStringUnicodeCharacter(char currentChar, StringBuilder output, StringBuilder number, out bool continueEscapedUnicodeChar)
        {
            if (HexDigits.IndexOf(currentChar) >= 0)
            {
                number.Append(currentChar);

                if (number.Length == 4)
                {
                    continueEscapedUnicodeChar = false;
                    return AddUnicodeChar(output, number, true);
                }

                // continune reading escaped Unicode character definition:
                continueEscapedUnicodeChar = true;
                return StringHelperStatusCode.Success;
            }

            continueEscapedUnicodeChar = false;
            return AddUnicodeChar(output, number, true);
        }

        /// <summary>
        /// Reads the string from given input stream.
        /// </summary>
        internal static StringHelperStatusCode ReadStringChars(IStringReader reader, StringBuilder output, StringBuilder escapedUnicodeNumberBuffer, bool errorOnNewLine, out int lastLine, out int lastOffset)
        {
            bool escape = false;
            bool unicodeNumber = false;

            if (escapedUnicodeNumberBuffer == null)
                escapedUnicodeNumberBuffer = new StringBuilder();

            lastLine = reader.Line;
            lastOffset = reader.LineOffset;

            do
            {
                if (!unicodeNumber)
                {
                    lastLine = reader.Line;
                    lastOffset = reader.LineOffset;
                }

                var currentChar = reader.ReadNext();

                // verify if not an invalid character was found in text:
                if (reader.IsEof)
                    return StringHelperStatusCode.UnexpectedEoF;
                if (errorOnNewLine && (currentChar == '\r' || currentChar == '\n'))
                    return StringHelperStatusCode.UnexpectedNewLine;

                if (unicodeNumber)
                {
                    StringHelperStatusCode result = ReadStringUnicodeCharacter(currentChar, output, escapedUnicodeNumberBuffer, out unicodeNumber);

                    // if parsing Unicode character failed, immediatelly stop!
                    if (result != StringHelperStatusCode.Success)
                        return result;

                    continue;
                }

                if (currentChar == '\\' && !escape)
                {
                    escape = true;
                }
                else
                {
                    if (escape)
                    {
                        switch (currentChar)
                        {
                            case 'n':
                                output.Append('\n');
                                break;
                            case 'r':
                                output.Append('\r');
                                break;
                            case 't':
                                output.Append('\t');
                                break;
                            case '/':
                                output.Append('/');
                                break;
                            case '\\':
                                output.Append('\\');
                                break;
                            case 'f':
                                output.Append('\f');
                                break;
                            case 'U':
                            case 'u':
                                unicodeNumber = true;
                                break;
                            case '"':
                                output.Append('"');
                                break;
                            case '\'':
                                output.Append('\'');
                                break;
                            default:
                                return StringHelperStatusCode.UnknownEscapedChar;
                        }

                        escape = false;
                    }
                    else
                    {
                        if (currentChar == '"')
                            break;

                        output.Append(currentChar);
                    }
                }
            }
            while (true);

            // as the string might finish with a Unicode character...
            if (unicodeNumber)
                return AddUnicodeChar(output, escapedUnicodeNumberBuffer, false);

            return StringHelperStatusCode.Success;
        }

        /// <summary>
        /// Reads the keyword definition chars from given input.
        /// </summary>
        internal static StringHelperStatusCode ReadKeywordChars(IStringReader reader, StringBuilder output)
        {
            do
            {
                var currentChar = reader.ReadNext();

                if (char.IsLetter(currentChar))
                {
                    output.Append(currentChar);
                }
                else
                    break;
            }
            while (true);

            return StringHelperStatusCode.Success;
        }

        internal static StringHelperStatusCode ReadCommentChars(IStringReader reader, bool multiline)
        {
            if (multiline)
            {
                char previousChar;
                char currentChar = '\0';
                do
                {
                    previousChar = currentChar;
                    currentChar = reader.ReadNext();

                    if (reader.IsEof)
                        return StringHelperStatusCode.UnexpectedEoF;

                    if (previousChar == '*' && currentChar == '/')
                        return StringHelperStatusCode.Success;
                }
                while (true);
            }
            else
            {
                do
                {
                    var currentChar = reader.ReadNext();

                    if (reader.IsEof)
                        return StringHelperStatusCode.UnexpectedEoF;

                    if (currentChar == '\r' || currentChar == '\n')
                        return StringHelperStatusCode.Success;
                }
                while (true);
            }
        }

        #endregion
    }

    internal enum StringHelperStatusCode
    {
        Success = 0,
        UnexpectedEoF,
        UnexpectedNewLine,
        UnknownEscapedChar,
        TooShortEscapedChar,
        TooLongEscapedChar
    }
}
