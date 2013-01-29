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
using CodeTitans.JSon.Objects;
using CodeTitans.JSon.ReaderHelpers.Factories;

namespace CodeTitans.JSon.ReaderHelpers
{
    /// <summary>
    /// Class that wraps creation of JSON-specific implementations based on read data.
    /// </summary>
    internal class JSonObjectFactory : IObjectFactory
    {
        /// <summary>
        /// Creates a proper FCL factory for given numer formatting parsing.
        /// </summary>
        internal static IObjectFactory Create(JSonReaderNumberFormat format)
        {
            switch(format)
            {
                case JSonReaderNumberFormat.Default:
                    return new JSonObjectFactory();
                case JSonReaderNumberFormat.AsInt32:
                    return new JSonObjectInt32Factory();
                case JSonReaderNumberFormat.AsInt64:
                    return new JSonObjectInt64Factory();
                case JSonReaderNumberFormat.AsDouble:
                    return new JSonObjectDoubleFactory();
                case JSonReaderNumberFormat.AsDecimal:
                    return new JSonObjectDecimalFactory();
                default:
                    throw new ArgumentOutOfRangeException("format", string.Concat("Unsupported number reading format (", format, ")"));
            }
        }

        private JSonObjectFactory()
        {
            Format = JSonReaderNumberFormat.Default;
        }

        protected JSonObjectFactory(JSonReaderNumberFormat format)
        {
            Format = format;
        }

        public JSonReaderNumberFormat Format
        {
            get;
            private set;
        }

        public object CreateArray(List<Object> data)
        {
            return new JSonArray(data);
        }

        public object CreateObject(Dictionary<String, Object> data)
        {
            return new JSonDictionary(data);
        }

        public object CreateKeyword(TokenDataString keyword)
        {
            return keyword.ValueAsJSonObject;
        }

        public object CreateString(string data)
        {
            return new JSonStringObject(data);
        }

        public virtual object CreateNumber(string data)
        {
            return ObjectFactoryHelper.ParseNumber(this, data);
        }

        public object CreateNumber(Int32 data)
        {
            return new JSonDecimalInt32Object(data);
        }

        public object CreateNumber(Int64 data)
        {
            return new JSonDecimalInt64Object(data);
        }

        public object CreateNumber(UInt64 data)
        {
            return new JSonDecimalUInt64Object(data);
        }

        public object CreateNumber(Double data)
        {
            return new JSonDecimalDoubleObject(data);
        }

        public object CreateNumber(Decimal data)
        {
            return new JSonDecimalDecimalObject(data);
        }
    }
}
