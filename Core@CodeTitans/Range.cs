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
using System.Globalization;
using System.Text;
using CodeTitans.Helpers;

namespace CodeTitans.Core
{
    /// <summary>
    /// Class that stores info about given range.
    /// </summary>
    public class Range : IEquatable<Range>
    {
        private readonly Int64 _location;
        private readonly UInt32 _length;

        /// <summary>
        /// Gets the starting from zero and with zero length range.
        /// </summary>
        public static readonly Range Zero = new Range(0, 0);

        /// <summary>
        /// Init constructor.
        /// </summary>
        public Range(Int32 location, UInt32 length)
        {
            _location = location;
            _length = length;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public Range(Int64 location, UInt32 length)
        {
            _location = location;
            _length = length;
        }

        #region Properties

        /// <summary>
        /// Gets the start location of the range.
        /// </summary>
        public Int64 Location
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the length of the range.
        /// </summary>
        public UInt32 Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets the smaller bound value.
        /// </summary>
        public Int64 LeftBound
        {
            get { return _location; }
        }

        /// <summary>
        /// Gets the bigger bound value.
        /// </summary>
        public Int64 RightBound
        {
            get { return _location + _length; }
        }

        #endregion

        /// <summary>
        /// Checks if given value belongs to the range.
        /// </summary>
        public bool Contains(Int32 value)
        {
            return value >= _location && value <= _location + _length;
        }

        /// <summary>
        /// Checks if given value belongs to the range.
        /// </summary>
        public bool Contains(UInt32 value)
        {
            return value >= _location && value <= _location + _length;
        }

        /// <summary>
        /// Checks if given value belongs to the range.
        /// </summary>
        public bool Contains(Int64 value)
        {
            return value >= _location && value <= _location + _length;
        }

        /// <summary>
        /// Gets the string representation of the object.
        /// </summary>
        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the string representation of the object for given format.
        /// </summary>
        public string ToString(string format)
        {
            return ToString(format, null);
        }

        /// <summary>
        /// Gets the string representation of the object for given format.
        /// Acceptable values: 
        /// </summary>
        public string ToString(string format, IFormatProvider provider)
        {
            if (provider == null)
                provider = CultureInfo.InvariantCulture;

            if (string.IsNullOrEmpty(format) || format == "D" || format == "d" )
                return string.Concat("{", _location.ToString(), ", ", _length.ToString(), "}");

            if (format == "P" || format == "p")
                return string.Concat("(", _location.ToString(), ", ", _length.ToString(), ")");

            if (format == "S" || format == "s")
                return string.Concat("[", _location.ToString(), ", ", _length.ToString(), "]");

            if (format == "C" || format == "c")
                return string.Format(provider, "{0}:{1}", _location, _length);

            if (format == "O" || format == "o")
                return _location.ToString();

            if (format == "E" || format == "e")
                return _length.ToString();

            throw new FormatException("Invalid format");
        }

        /// <summary>
        /// Compares current Range to given object.
        /// </summary>
        public override bool Equals(object obj)
        {
            Range other = (Range)obj;

            return Equals(other);
        }

        /// <summary>
        /// Compare for equality.
        /// </summary>
        public static bool operator ==(Range a, Range b)
        {
            // if both are null or the same instance:
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Compare for inequality.
        /// </summary>
        public static bool operator !=(Range a, Range b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Verifies if given Range is equal to current one.
        /// </summary>
        public bool Equals(Range other)
        {
            if (other == null)
                return false;

            return _location == other.Location && _length == other.Length;
        }

        /// <summary>
        /// Gets the hash value for this object.
        /// </summary>
        public override int GetHashCode()
        {
            return (int)(_location ^ _length);
        }

        /// <summary>
        /// Parses the text to a range object. It tries all known formats.
        /// </summary>
        public static Range Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new FormatException("empty text");

            IStringReader reader = new StringReaderWrapper(text);
            StringBuilder buffer = new StringBuilder();
            Int64 location = 0;
            UInt32 length = 0;
            char expectedEnding = '\0';
            bool bracketRead = false;
            bool commaRead = false;
            bool colonRead = false;
            int numbersRead = 0;

            while (!reader.IsEof)
            {
                StringHelper.ReadWhiteChars(reader);

                if (reader.CurrentChar == '-' || reader.CurrentChar == '+' || char.IsDigit(reader.CurrentChar))
                {
                    if (numbersRead >= 2)
                        throw new FormatException("Too many numbers");

                    buffer.Remove(0, buffer.Length);
                    buffer.Append(reader.CurrentChar);
                    StringHelper.ReadIntegerNumberChars(reader, buffer);

                    if (numbersRead == 0)
                    {
                        if (!NumericHelper.TryParseInt64(buffer.ToString(), out location))
                            throw new FormatException("Invalid location number definition");
                    }

                    if (numbersRead == 1)
                    {
                        if (!NumericHelper.TryParseUInt32(buffer.ToString(), out length))
                            throw new FormatException("Invalid length number definition");
                    }

                    numbersRead++;
                }

                if (reader.CurrentChar == '{' || reader.CurrentChar == '[' || reader.CurrentChar == '(')
                {
                    if (expectedEnding == '\0')
                    {
                        if (bracketRead)
                            throw new FormatException("Already read one bracket pair");

                        expectedEnding = reader.CurrentChar;
                    }
                    else
                        throw new FormatException("Reapeated startup bracket");
                    continue;
                }

                if (reader.CurrentChar == '}' || reader.CurrentChar == ']' || reader.CurrentChar == ')')
                {
                    if (expectedEnding == '\0')
                    {
                        throw new FormatException("Missing opening bracket");
                    }
                    if ((expectedEnding == '{' && reader.CurrentChar != '}')
                        || (expectedEnding == '[' && reader.CurrentChar != ']')
                        || (expectedEnding == '(' && reader.CurrentChar != ')'))
                        throw new FormatException("Not matching closing bracket");

                    expectedEnding = '\0';
                    bracketRead = true;

                    if (numbersRead != 2)
                        throw new FormatException("Should read two numbers, before closing bracket");
                    continue;
                }

                if (reader.CurrentChar == ':')
                {
                    if (colonRead)
                        throw new FormatException("Colon already read");

                    colonRead = true;
                    continue;
                }

                if (reader.CurrentChar == ',')
                {
                    if (commaRead)
                        throw new FormatException("Comma already read");
                    commaRead = true;
                    continue;
                }

                if (!char.IsWhiteSpace(reader.CurrentChar) && !reader.IsEof)
                    throw new FormatException("Unrecognized char found");
            }

            // final validation:
            if (numbersRead < 2)
                throw new FormatException("Too few numbers read");
            if (expectedEnding != '\0')
                throw new FormatException("Missing closing bracket");

            return new Range(location, length);
        }

        /// <summary>
        /// Tries to parse value from any known format.
        /// If succeeds, then returned is 'true' and 'value' contains the parsed range.
        /// </summary>
        public static bool TryParse(string text, out Range value)
        {
            try
            {
                value = Parse(text);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }
    }
}
