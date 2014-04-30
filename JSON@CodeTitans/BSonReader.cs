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
using CodeTitans.Helpers;
using CodeTitans.JSon.ReaderHelpers;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Binary JSON standard reader.
    /// Full specification can be found at: http://bsonspec.org
    /// </summary>
    public sealed class BSonReader : IJSonReader
    {
        enum BSonItemType : byte
        {
            EndOfDocument = 0,
            Double = 1,
            String = 2,
            Object = 3,
            Array = 4,
            Binary = 5,
            /// <summary>
            /// Deprecated
            /// </summary>
            Undefined = 6,
            /// <summary>
            /// 12-bytes
            /// </summary>
            ObjectID = 7,
            Boolean = 8,
            /// <summary>
            /// In UTC format
            /// </summary>
            DateTime = 9,
            Null = 10,
            RegExp = 11,
            /// <summary>
            /// Deprecated
            /// </summary>
            DbPointer = 12,
            /// <summary>
            /// JavaScript code
            /// </summary>
            JavaScript = 13,
            /// <summary>
            /// Deprecated
            /// </summary>
            Symbol = 14,
            /// <summary>
            /// JavaScript code with scope
            /// </summary>
            JavaScriptScoped = 15,
            Int32 = 16,
            /// <summary>
            /// MongoDB replication and sharding; first 4-bytes are an increment, latter 4-bytes are a timestamp
            /// </summary>
            Timestamp = 17,
            Int64 = 18,
            MinKey = 0xFF,
            MaxKey = 0x7F
        }

        private IObjectFactory _factory;
        private IBinaryReader _input;

        public BSonReader(BinaryReader input)
        {
            _input = BinaryHelper.CreateReader(input);
        }

        public BSonReader(byte[] input)
        {
            _input = BinaryHelper.CreateReader(input);
        }

        public BSonReader()
        {
        }

        public void SetSource(BinaryReader input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            _input = BinaryHelper.CreateReader(input);
        }

        public void SetSource(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            _input = BinaryHelper.CreateReader(input);
        }

        private object ReadItem(BSonItemType type, out string key)
        {
            int length;
            int dataOffset;

            key = _input.ReadStringUTF8(int.MaxValue);
            dataOffset = _input.Offset;
            switch (type)
            {
                case BSonItemType.Double:
                    return _factory.CreateNumber(_input.ReadDouble());

                case BSonItemType.String:
                case BSonItemType.JavaScript:
                case BSonItemType.Symbol:
                    length = _input.ReadInt32();
                    return _factory.CreateString(_input.ReadStringUTF8(length));

                case BSonItemType.Object:
                    return ReadDocument();

                case BSonItemType.Array:
                    return ReadArray();

                case BSonItemType.Binary:
                    throw new FormatException("Unsupported 'binary' field at " + dataOffset);

                case BSonItemType.Undefined:
                    throw new FormatException("Unsupported 'undefined' field at " + dataOffset);

                case BSonItemType.ObjectID:
                    throw new FormatException("Unsupported 'object-id' field at " + dataOffset);

                case BSonItemType.Boolean:
                    return _factory.CreateKeyword(_input.ReadByte() > 0 ? JSonReader.TrueTokenData : JSonReader.FalseTokenData);

                case BSonItemType.DateTime:
                    return _factory.CreateNumber(_input.ReadUInt64());

                case BSonItemType.Null:
                    return _factory.CreateKeyword(JSonReader.FalseTokenData);

                case BSonItemType.RegExp:
                    return ReadRegExp();

                case BSonItemType.DbPointer:
                    throw new FormatException("Unsupported 'DbPointer' field at " + dataOffset);

                case BSonItemType.JavaScriptScoped:
                    return ReadJavaScriptScoped();

                case BSonItemType.Int32:
                    return _factory.CreateNumber(_input.ReadInt32());

                case BSonItemType.Timestamp:
                    return _factory.CreateNumber(_input.ReadInt64());

                case BSonItemType.Int64:
                    return _factory.CreateNumber(_input.ReadInt64());

                default:
                    throw new FormatException("Unsupported '" + type + "' field at " + dataOffset);
            }
        }

        private object ReadRegExp()
        {
            var resultArray = new List<object>();

            resultArray.Add(_input.ReadStringUTF8(int.MaxValue));
            resultArray.Add(_input.ReadStringUTF8(int.MaxValue));

            return _factory.CreateArray(resultArray);
        }

        private object ReadJavaScriptScoped()
        {
            var resultArray = new List<object>();

            _input.ReadInt32(); // skip length of the entire structure

            int length = _input.ReadInt32();
            resultArray.Add(_input.ReadStringUTF8(length));
            resultArray.Add(ReadDocument());

            return _factory.CreateArray(resultArray);
        }

        private object ReadDocument()
        {
            // validate data length:
            var length = _input.ReadUInt32();

            if (length == 0)
                return null;

            // decode data:
            var resultObject = new Dictionary<string, object>();
            BSonItemType type;
            string key;
            object value;
            int prevOffset;

            while ((type = (BSonItemType) _input.ReadByte()) != BSonItemType.EndOfDocument)
            {
                prevOffset = _input.Offset;
                value = ReadItem(type, out key);

                if (string.IsNullOrEmpty(key))
                    throw new FormatException("Missing key, when parsing BSON document at " + prevOffset);

                resultObject[key] = value;
            }

            return _factory.CreateObject(resultObject);
        }

        private object ReadArray()
        {
            // validate data length:
            var length = _input.ReadUInt32();

            if (length == 0)
                return null;

            // decode data:
            var resultArray = new List<object>();
            BSonItemType type;
            string key;
            object value;

            while ((type = (BSonItemType)_input.ReadByte()) != BSonItemType.EndOfDocument)
            {
                value = ReadItem(type, out key);
                resultArray.Add(value);
            }

            return _factory.CreateArray(resultArray);
        }

        private object ReadInput()
        {
            if (_input == null)
                throw new InvalidOperationException("Missing input. Please call SetSource() or use different constructor before parsing data.");
            if (_factory == null)
                throw new InvalidOperationException("Missing internal object's factory for parsed data");

            // format review:
            //  [length:int] [document] [\x00:byte]
            //
            // while [document] is a sequence of:
            //  [type:byte] [name:utf8-string-with-zero] [value:optional]

            if (!_input.IsEmpty && !_input.IsEof)
            {
                return ReadDocument();
            }

            return null;
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        public object Read()
        {
            _factory = FclObjectFactory.Create(JSonReaderNumberFormat.Default);
            return ReadInput();
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        public object Read(JSonReaderNumberFormat format)
        {
            // numerical formats are ignored as the BSON-spec defines them clearly:
            return Read();
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonObject ReadAsJSonObject()
        {
            _factory = JSonObjectFactory.Create(JSonReaderNumberFormat.Default);
            return ReadInput() as IJSonObject;
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonObject ReadAsJSonObject(JSonReaderNumberFormat format)
        {
            // numerical formats are ignored as the BSON-spec defines them clearly:
            return ReadAsJSonObject();
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonMutableObject ReadAsJSonMutableObject()
        {
            _factory = JSonMutableObjectFactory.Create(JSonReaderNumberFormat.Default);
            return ReadInput() as IJSonMutableObject;
        }

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        public IJSonMutableObject ReadAsJSonMutableObject(JSonReaderNumberFormat format)
        {
            // numerical formats are ignored as the BSON-spec defines them clearly:
            return ReadAsJSonMutableObject();
        }
    }
}
