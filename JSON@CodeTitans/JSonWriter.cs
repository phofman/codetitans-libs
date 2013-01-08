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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using CodeTitans.Helpers;
using CodeTitans.JSon.WriterHelpers;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Class that allows to construct the JSON objects and writes them into given <see cref="TextWriter"/> output.
    /// </summary>
    public sealed class JSonWriter : IJSonWriter
    {
        private readonly TextWriter _output;
        private readonly Stack<JSonWriterTokenInfo> _tokens;
        private Int32 _level;
        private readonly bool _closeOutput;
        private ArrayWriter _arrayHelper;
        private ObjectWriter _objectHelper;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriter(TextWriter output)
            : this(output, false)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriter(TextWriter output, bool indent)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            _output = output;
            _tokens = new Stack<JSonWriterTokenInfo>();
            _level = 1;
            Indent = indent;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriter(StringBuilder output)
            : this(output, false)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriter(StringBuilder output, bool indent)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            _output = new StringWriter(output, CultureInfo.InvariantCulture);
            _tokens = new Stack<JSonWriterTokenInfo>();
            _level = 1;
            _closeOutput = true;
            Indent = indent;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonWriter(bool indent)
        {
            Indent = indent;
            _output = new StringWriter(CultureInfo.InvariantCulture);
            _tokens = new Stack<JSonWriterTokenInfo>();
            _level = 1;
            _closeOutput = true;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonWriter()
        {
            _output = new StringWriter(CultureInfo.InvariantCulture);
            _tokens = new Stack<JSonWriterTokenInfo>();
            _level = 1;
            _closeOutput = true;
        }

        /// <summary>
        /// Get the JSON string representation stored inside.
        /// Before accessing it, use <seealso cref="IsValid"/> property to validate
        /// if content has a JSON valid structure.
        /// </summary>
        /// <returns>String representing JSON object.</returns>
        public override string ToString()
        {
            return _output != null ? _output.ToString() : null;
        }

        /// <summary>
        /// Gets or sets the indication, if generated content should be indented.
        /// The setting take effect from the next writing, so the previously
        /// written elements are no affected.
        /// </summary>
        public bool Indent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the indication, if generated enumerables (JSON arrays and JSON dictionaries)
        /// should have a whitespace after a comma (,) or after semicolon (:).
        /// </summary>
        public bool CompactEnumerables
        {
            get;
            set;
        }

        /// <summary>
        /// Checks if given instance of JSON writer contains a valid content.
        /// </summary>
        public bool IsValid
        {
            get { return _tokens.Count == 0 && _output != null; }
        }

        #region Verification Methods

        private JSonWriterTokenInfo VerifyTopTokenEqualToAndDequeue(JSonWriterTokenType expected)
        {
            JSonWriterTokenType currentToken = JSonWriterTokenType.Nothing;
            JSonWriterTokenInfo t = null;

            if (_tokens.Count > 0)
            {
                t = _tokens.Pop();
                currentToken = t.TokenType;
            }

            if (currentToken != expected)
                throw new JSonWriterException(expected, currentToken);

            return t;
        }

        private JSonWriterTokenInfo VerifyTopTokenEqualTo(JSonWriterTokenType expected)
        {
            if (_tokens.Count == 0)
                throw new JSonWriterException(expected, JSonWriterTokenType.Nothing);

            JSonWriterTokenType currentToken = _tokens.Peek().TokenType;

            if (currentToken != expected)
                throw new JSonWriterException(expected, currentToken);

            if (_tokens.Count > 0)
                return _tokens.Peek();

            return null;
        }

        private JSonWriterTokenInfo VerifyTopTokenEqualTo(JSonWriterTokenType expected1, JSonWriterTokenType expected2)
        {
            JSonWriterTokenType currentToken = JSonWriterTokenType.Nothing;

            if (_tokens.Count > 0)
                currentToken = _tokens.Peek().TokenType;

            if (currentToken != expected1 && currentToken != expected2)
            {
                if (currentToken != expected1)
                    throw new JSonWriterException(expected1, currentToken);
                throw new JSonWriterException(expected2, currentToken);
            }

            if (_tokens.Count > 0)
                return _tokens.Peek();

            return null;
        }

        private JSonWriterTokenInfo VerifyTopTokenDifferentThan(JSonWriterTokenType expected)
        {
            if (_tokens.Count > 0)
            {
                JSonWriterTokenType currentToken = _tokens.Peek().TokenType;

                if (currentToken == expected)
                    throw new JSonWriterException(expected, currentToken, false);

                return _tokens.Peek();
            }

            return null;
        }

        #endregion

        #region Object Serialization Methods

        /// <summary>
        /// Writes an object start.
        /// </summary>
        public void WriteObjectBegin()
        {
            // object can't be directly embedded inside another object:
            JSonWriterTokenInfo currentToken = VerifyTopTokenDifferentThan(JSonWriterTokenType.Object);
            _tokens.Push(new JSonWriterTokenInfo(JSonWriterTokenType.Object, _level++));

            if (currentToken != null)
                currentToken.AddItem(_output, currentToken.TokenType != JSonWriterTokenType.MemberValue && Indent, CompactEnumerables);

            _output.Write('{');
        }

        /// <summary>
        /// Writes an object end.
        /// </summary>
        public void WriteObjectEnd()
        {
            JSonWriterTokenInfo currentToken = VerifyTopTokenEqualToAndDequeue(JSonWriterTokenType.Object);

            // if this object was added as a value in
            if (_tokens.Count > 0 && _tokens.Peek().TokenType == JSonWriterTokenType.MemberValue)
            {
                _tokens.Pop();
            }

            if (currentToken != null)
                currentToken.RemoveItem(_output, Indent);

            _level--;
            _output.Write('}');
        }

        /// <summary>
        /// Writes automatically object begin/end statements, if call to this method is enclosed in 'using' statement.
        /// </summary>
        public IJSonWriterObjectItem WriteObject()
        {
            if (_objectHelper == null)
                _objectHelper = new ObjectWriter(this);

            return _objectHelper.WriteObjectBegin();
        }

        /// <summary>
        /// Writes automatically embedded object begin/end statements for parent object's member name, if call to this method is enclosed in 'using' statement.
        /// </summary>
        public IJSonWriterObjectItem WriteObject(string name)
        {
            if (_objectHelper == null)
                _objectHelper = new ObjectWriter(this);

            WriteMember(name);
            return _objectHelper.WriteObjectBegin();
        }

        /// <summary>
        /// Writes an array start.
        /// </summary>
        public void WriteArrayBegin()
        {
            JSonWriterTokenInfo currentToken = _tokens.Count > 0 ? _tokens.Peek() : null;

            _tokens.Push(new JSonWriterTokenInfo(JSonWriterTokenType.Array, _level++));

            if (currentToken != null)
                currentToken.AddItem(_output, currentToken.TokenType != JSonWriterTokenType.MemberValue && Indent, CompactEnumerables);

            _output.Write('[');
        }

        /// <summary>
        /// Writes an array end.
        /// </summary>
        public void WriteArrayEnd()
        {
            JSonWriterTokenInfo currentToken = VerifyTopTokenEqualToAndDequeue(JSonWriterTokenType.Array);

            // if this array was added as a value in
            if (_tokens.Count > 0 && _tokens.Peek().TokenType == JSonWriterTokenType.MemberValue)
            {
                _tokens.Pop();
            }

            if (currentToken != null)
                currentToken.RemoveItem(_output, Indent);

            _level--;
            _output.Write(']');
        }

        /// <summary>
        /// Writes automatically array begin/end statements, if call to this method is enclosed in 'using' statement.
        /// </summary>
        public IJSonWriterArrayItem WriteArray()
        {
            if (_arrayHelper == null)
                _arrayHelper = new ArrayWriter(this);

            return _arrayHelper.WriteArrayBegin();
        }

        /// <summary>
        /// Writes automatically embedded array begin/end statements for parent object's member name, if call to this method is enclosed in 'using' statement.
        /// </summary>
        public IJSonWriterArrayItem WriteArray(string name)
        {
            if (_arrayHelper == null)
                _arrayHelper = new ArrayWriter(this);

            WriteMember(name);
            return _arrayHelper.WriteArrayBegin();
        }

        private void WriteMemberInternal(string name, string value, bool quoteValue)
        {
            JSonWriterTokenInfo currentToken = VerifyTopTokenEqualTo(JSonWriterTokenType.Object);

            if (currentToken != null)
                currentToken.AddItem(_output, Indent, CompactEnumerables);

            _output.Write('"');
            StringHelper.WriteSecureString(_output, name);
            _output.Write('"');

            if (CompactEnumerables)
                _output.Write(':');
            else
                _output.Write(": ");

            if (quoteValue)
                _output.Write('"');

            _output.Write(value);

            if (quoteValue)
                _output.Write('"');
        }

        /// <summary>
        /// Writes a 'null' text as a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, DBNull value)
        {
            WriteMemberInternal(name, JSonReader.NullString, false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// If 'null' value is passed it will emit the JSON 'null' for given member.
        /// </summary>
        public void WriteMember(string name, String value)
        {
            if (value == null)
                WriteMemberInternal(name, JSonReader.NullString, false);
            else
                WriteMemberInternal(name, StringHelper.GetSecureString(value), true);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Int32 value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, UInt32 value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Int64 value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, UInt64 value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Single value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Double value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Decimal value)
        {
            WriteMemberInternal(name, value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        public void WriteMember(string name, DateTime value)
        {
            // write as universal (GMT/Zulu) time:
            WriteMemberInternal(name, ToString(value, JSonWriterDateFormat.Default), true);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        public void WriteMember(string name, DateTime value, JSonWriterDateFormat format)
        {
            WriteMemberInternal(name, ToString(value, format), ToQuoteValue(format));
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, TimeSpan value)
        {
            WriteMemberInternal(name, ToString(value), true);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Boolean value)
        {
            WriteMemberInternal(name, ToString(value), false);
        }

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        public void WriteMember(string name, Guid value)
        {
            WriteMemberInternal(name, StringHelper.GetSecureString(ToString(value)), true);
        }

        /// <summary>
        /// Writes 'null' JSON value.
        /// </summary>
        public void WriteMemberNull(string name)
        {
            WriteMemberInternal(name, JSonReader.NullString, false);
        }

        /// <summary>
        /// Writes a member only to an object.
        /// It requires to be called before writing an object (or array) inside current object.
        /// Writing values after call to this function is also allowed, however it is not recommented
        /// due to number of checks performed. The better performance can be achieved
        /// if other overloaded functions are used.
        /// </summary>
        public void WriteMember(string name)
        {
            JSonWriterTokenInfo currentToken = VerifyTopTokenEqualTo(JSonWriterTokenType.Object);

            currentToken.AddItem(_output, Indent, CompactEnumerables);

            _tokens.Push(new JSonWriterTokenInfo(JSonWriterTokenType.MemberValue, _level));
            _output.Write('"');
            StringHelper.WriteSecureString(_output, name);
            _output.Write('"');
            if (CompactEnumerables)
                _output.Write(':');
            else
                _output.Write(": ");
        }

        private void WriteValueInternal(string value, bool quoteValue)
        {
            // allow adding value item, when it's just a simple writing without wrapping
            // in array or object:
            if (_tokens.Count == 0)
            {
                // push dummy item on tokens stack, so that on the next try, verification will fail,
                // only one value item can be added!
                _tokens.Push(new JSonWriterTokenInfo(JSonWriterTokenType.Nothing, -1));
                if (quoteValue)
                    _output.Write('"');
                _output.Write(value);
                if (quoteValue)
                    _output.Write('"');
                return;
            }

            JSonWriterTokenInfo currentToken = VerifyTopTokenEqualTo(JSonWriterTokenType.Array, JSonWriterTokenType.MemberValue);

            currentToken.AddItem(_output, currentToken.TokenType != JSonWriterTokenType.MemberValue && Indent, CompactEnumerables);

            if (currentToken.TokenType == JSonWriterTokenType.MemberValue)
            {
                _tokens.Pop();
            }

            if (quoteValue)
                _output.Write('"');
            _output.Write(value);
            if (quoteValue)
                _output.Write('"');
        }

        /// <summary>
        /// Writes a 'null' value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(DBNull value)
        {
            WriteValueInternal(JSonReader.NullString, false);
        }

        /// <summary>
        /// Writes string value.
        /// It can be used only as an array element or value for object member.
        /// If 'null' value is passed it will emit the JSON 'null' for given member.
        /// </summary>
        public void WriteValue(String value)
        {
            if (value == null)
                WriteValueInternal(JSonReader.NullString, false);
            else
                WriteValueInternal(StringHelper.GetSecureString(value), true);
        }

        /// <summary>
        /// Writes integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Int32 value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes unsigned integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(uint value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Int64 value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes unsigned long integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(ulong value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes decimal value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Single value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes decimal value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Double value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes decimal value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Decimal value)
        {
            WriteValueInternal(value.ToString(CultureInfo.InvariantCulture), false);
        }

        /// <summary>
        /// Writes DateTime value.
        /// It can be used only as an array element or value for object member.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        public void WriteValue(DateTime value)
        {
            WriteValueInternal(ToString(value, JSonWriterDateFormat.Default), true);
        }

        /// <summary>
        /// Writes DateTime value.
        /// It can be used only as an array element or value for object member.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        public void WriteValue(DateTime value, JSonWriterDateFormat format)
        {
            WriteValueInternal(ToString(value, format), ToQuoteValue(format));
        }

        /// <summary>
        /// Writes TimeSpan value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(TimeSpan value)
        {
            WriteValueInternal(ToString(value), true);
        }

        /// <summary>
        /// Writes Boolean value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Boolean value)
        {
            WriteValueInternal(ToString(value), false);
        }

        /// <summary>
        /// Writes Guid value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValue(Guid value)
        {
            WriteValueInternal(StringHelper.GetSecureString(ToString(value)), true);
        }

        /// <summary>
        /// Writes 'null' value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        public void WriteValueNull()
        {
            WriteValueInternal(JSonReader.NullString, false);
        }

        /// <summary>
        /// Writes a value based on dynamic type checking.
        /// It can be then a string, number, embedded dictionary or array.
        /// </summary>
        public void WriteValueDynamic(object value)
        {
            WriteEmbeddedValue(value);
        }

        #endregion

        #region Data Serialization As String

        /// <summary>
        /// Converts DateTime to String.
        /// </summary>
        internal static String ToString(DateTime value)
        {
            return ToString(value, JSonWriterDateFormat.Default);
        }

        /// <summary>
        /// Converts DateTime to String.
        /// </summary>
        internal static String ToString(DateTime value, JSonWriterDateFormat format)
        {
            DateTime valueAsUniversal = value.ToUniversalTime();
            long milliseconds = 0;

            switch (format)
            {
                case JSonWriterDateFormat.Default:
                    return valueAsUniversal.ToString("u", CultureInfo.InvariantCulture);

                case JSonWriterDateFormat.JavaScript:
                    milliseconds = (valueAsUniversal.Ticks - DateTimeHelper.TicksAt1970) / TimeSpan.TicksPerMillisecond;

                    // is before 1970-01-01?
                    if (milliseconds < 0)
                    {
                        // use a direct year-month-day constructor, if no hour-minute-second-millisecond set:
                        if ((valueAsUniversal.Ticks % TimeSpan.TicksPerDay) == 0)
                            return string.Concat("\\/Date(", valueAsUniversal.Year, ",", valueAsUniversal.Month, ",", valueAsUniversal.Day, ")\\/");

                        // or full date+time:
                        return string.Concat("\\/Date(", valueAsUniversal.Year, ",", valueAsUniversal.Month, ",", valueAsUniversal.Day, ",",
                                             valueAsUniversal.Hour, ",", valueAsUniversal.Minute, ",", valueAsUniversal.Second, ",", valueAsUniversal.Millisecond, ")\\/");
                    }

                    // or by default only miliseconds if date >= 1970-01-01:
                    return string.Concat("\\/Date(", milliseconds, ")\\/");

                case JSonWriterDateFormat.UnixEpochSeconds:

                    // is before 1970-01-01?
                    if (valueAsUniversal.Ticks < DateTimeHelper.TicksAt1970)
                        throw new FormatException("Date is too early, should be after 1970-01-01");

                    long seconds = (valueAsUniversal.Ticks - DateTimeHelper.TicksAt1970) / TimeSpan.TicksPerSecond;
                    return seconds.ToString();

                case JSonWriterDateFormat.UnixEpochMilliseconds:

                    // is before 1970-01-01?
                    if (valueAsUniversal.Ticks < DateTimeHelper.TicksAt1970)
                        throw new FormatException("Date is too early, should be after 1970-01-01");

                    milliseconds = (valueAsUniversal.Ticks - DateTimeHelper.TicksAt1970) / TimeSpan.TicksPerMillisecond;
                    return milliseconds.ToString();

                case JSonWriterDateFormat.Ticks:
                    return valueAsUniversal.Ticks.ToString();

                default:
                    throw new FormatException(string.Concat("Unsupported format specified (", format, ")"));
            }
        }

        /// <summary>
        /// Gets an indication, if given data-time format requires to be serialized as number or quoted string.
        /// </summary>
        internal static bool ToQuoteValue(JSonWriterDateFormat format)
        {
            return format == JSonWriterDateFormat.Default || format == JSonWriterDateFormat.JavaScript;
        }

        /// <summary>
        /// Converts TimeSpan to String.
        /// </summary>
        internal static String ToString(TimeSpan value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts Boolean to String.
        /// </summary>
        internal static String ToString(Boolean value)
        {
            return value ? JSonReader.TrueString : JSonReader.FalseString;
        }

        /// <summary>
        /// Converts Guid to String.
        /// </summary>
        internal static String ToString(Guid value)
        {
            return value.ToString("D");
        }

        #endregion

        #region Whole Object serialization

        private void WriteEmbeddedValue(object oValue)
        {
            // check if this is a struct:
            if (oValue is Single)
            {
                WriteValue((Single)oValue);
                return;
            }
            if (oValue is Double)
            {
                WriteValue((Double)oValue);
                return;
            }
            if (oValue is Decimal)
            {
                WriteValue((Decimal) oValue);
                return;
            }
            if (oValue is DateTime)
            {
                WriteValue((DateTime)oValue);
                return;
            }
            if (oValue is TimeSpan)
            {
                WriteValue((TimeSpan)oValue);
                return;
            }
            if (oValue is Boolean)
            {
                WriteValue((Boolean)oValue);
                return;
            }
            // check if this is a class object:
            if (oValue == null)
            {
                WriteValueNull();
                return;
            }
            if (oValue is DBNull)
            {
                WriteValueNull();
                return;
            }
            if (oValue is String || oValue is Char)
            {
                WriteValue(oValue.ToString());
                return;
            }
            if (oValue is Guid)
            {
                WriteValue((Guid)oValue);
                return;
            }
            if (oValue is Enum)
            {
                WriteValue(oValue.ToString());
                return;
            }

            // does it implement the dedicated serialization interface?
            IJSonWritable jValue = oValue as IJSonWritable;

            if (jValue != null)
            {
                jValue.Write(this);
                return;
            }

            // is it marked with serialization attribute?
            Type oType = oValue.GetType();
            JSonSerializableAttribute jsonAttribute = ReflectionHelper.GetCustomAttribute<JSonSerializableAttribute>(oType);

            if (jsonAttribute != null)
            {
                WriteAttributedObject(oValue, oType, jsonAttribute);
                return;
            }

            // is a dictionary?
            IDictionary dict = oValue as IDictionary;

            if (dict != null)
            {
                Write(dict);
                return;
            }

            // is an array of elements?
            IEnumerable array = oValue as IEnumerable;

            if (array != null)
                Write(array);
            else
                WriteValueInternal(oValue.ToString(), false);
        }

        /// <summary>
        /// Writes enumerable collection as a JSON array.
        /// </summary>
        public void Write(IEnumerable array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            // already implements serialization interface?
            IJSonWritable jValue = array as IJSonWritable;

            if (jValue != null)
            {
                jValue.Write(this);
                return;
            }

            // is marked with serialization attribute?
            Type oType = array.GetType();
            JSonSerializableAttribute jsonAttribute = ReflectionHelper.GetCustomAttribute<JSonSerializableAttribute>(oType);

            if (jsonAttribute != null)
            {
                WriteAttributedObject(array, oType, jsonAttribute);
                return;
            }

            WriteArrayBegin();
            foreach (object value in array)
            {
                WriteEmbeddedValue(value);
            }
            WriteArrayEnd();
        }

        /// <summary>
        /// Writes a dictionary as a JSON object.
        /// </summary>
        public void Write(IDictionary dictionary)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            // already implements serialization interface?
            IJSonWritable jValue = dictionary as IJSonWritable;

            if (jValue != null)
            {
                jValue.Write(this);
                return;
            }

            // is marked with serialization attribute?
            Type oType = dictionary.GetType();
            JSonSerializableAttribute jsonAttribute = ReflectionHelper.GetCustomAttribute<JSonSerializableAttribute>(oType);

            if (jsonAttribute != null)
            {
                WriteAttributedObject(dictionary, oType, jsonAttribute);
                return;
            }

            WriteObjectBegin();
            foreach (DictionaryEntry entry in dictionary)
            {
                WriteMember(entry.Key.ToString());
                WriteEmbeddedValue(entry.Value);
            }
            WriteObjectEnd();
        }

        /// <summary>
        /// Writes a serializable object as JSON string.
        /// </summary>
        public void Write(IJSonWritable o)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            o.Write(this);
        }

        private void WriteAttributedMember(string itemName, object value, JSonMemberAttribute attr)
        {
            if (attr == null || !attr.SkipWhenNull || (attr.SkipWhenNull && value != null))
            {
                WriteMember(attr == null || string.IsNullOrEmpty(attr.Name) ? itemName : attr.Name);
                WriteEmbeddedValue(value);
            }
        }

        private void WriteAttributedObject(object o, Type oType, JSonSerializableAttribute jsonAttribute)
        {
            if (o == null)
                throw new ArgumentNullException("o");
            if (oType == null)
                throw new ArgumentNullException("oType");
            if (jsonAttribute == null)
                throw new ArgumentNullException("jsonAttribute");

            // get members that will be serialized:
#if WINDOWS_STORE
            IEnumerable<FieldInfo> fieldMembers = ReflectionHelper.GetFields(oType);
            IEnumerable<PropertyInfo> propertyMembers = ReflectionHelper.GetProperties(oType);
#else
            IEnumerable<FieldInfo> fieldMembers = ReflectionHelper.GetFields(oType, jsonAttribute.Flags);
            IEnumerable<PropertyInfo> propertyMembers = ReflectionHelper.GetProperties(oType, jsonAttribute.Flags);
#endif

            WriteObjectBegin();

            // serialize all fields:
            foreach (FieldInfo f in fieldMembers)
            {
                if (!f.IsLiteral)
                {
                    JSonMemberAttribute attr = ReflectionHelper.GetCustomAttribute<JSonMemberAttribute>(f);
                    if (attr != null || jsonAttribute.AllowAllFields)
                    {
                        WriteAttributedMember(f.Name, f.GetValue(o), attr);
                    }
                }
            }

            // serialize all properties:
            foreach (PropertyInfo p in propertyMembers)
            {
                // there must be a getter defined:
                if (p.CanRead)
                {
                    JSonMemberAttribute attr = ReflectionHelper.GetCustomAttribute<JSonMemberAttribute>(p);
                    if (attr != null || jsonAttribute.AllowAllProperties)
                    {
                        WriteAttributedMember(p.Name, p.GetValue(o, null), attr);
                    }
                }
            }

            WriteObjectEnd();
        }

        /// <summary>
        /// Writes whole object represented as dictionary or enumerable collection.
        /// </summary>
        public void Write(object o)
        {
            // try to access object as IJSonWritable class:
            IJSonWritable jValue = o as IJSonWritable;

            if (jValue != null)
            {
                Write(jValue);
                return;
            }

            if (o != null)
            {
                Type oType = o.GetType();
                JSonSerializableAttribute jsonAttribute = ReflectionHelper.GetCustomAttribute<JSonSerializableAttribute>(oType);

                if (jsonAttribute != null)
                {
                    WriteAttributedObject(o, oType, jsonAttribute);
                    return;
                }
            }

            // try to access object as dictionary:
            IDictionary dict = o as IDictionary;

            if (dict != null)
            {
                Write(dict);
                return;
            }

            // try to access object as enumerable:
            IEnumerable array = o as IEnumerable;

            if (array != null)
            {
                Write(array);
                return;
            }

            throw new ArgumentException("Invalid object type to serialize to JSON", "o");
        }

        #endregion

        #region IDispose Pattern

        /// <summary>
        /// Finalizer.
        /// </summary>
        ~JSonWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// Closes the output and releases internal resources.
        /// </summary>
        public void Close()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_output != null && _closeOutput)
                {
#if WINDOWS_STORE
                    _output.Dispose();
#else
                    _output.Close();
#endif
                }
            }
        }

        /// <summary>
        /// Releases managed and native resources.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        #endregion
    }
}
