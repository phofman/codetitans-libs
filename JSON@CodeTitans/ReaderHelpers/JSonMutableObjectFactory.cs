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
using CodeTitans.JSon.Objects.Mutable;
using CodeTitans.JSon.ReaderHelpers.Factories;

namespace CodeTitans.JSon.ReaderHelpers
{
    /// <summary>
    /// Class that wraps creation of JSON-specific implementations based on read data.
    /// </summary>
    internal class JSonMutableObjectFactory : IObjectFactory
    {
        /// <summary>
        /// Creates a proper FCL factory for given numer formatting parsing.
        /// </summary>
        internal static IObjectFactory Create(JSonReaderNumberFormat format)
        {
            switch (format)
            {
                case JSonReaderNumberFormat.Default:
                    return new JSonMutableObjectFactory();
                case JSonReaderNumberFormat.AsInt32:
                    return new JSonMutableObjectInt32Factory();
                case JSonReaderNumberFormat.AsInt64:
                    return new JSonMutableObjectInt64Factory();
                case JSonReaderNumberFormat.AsDouble:
                    return new JSonMutableObjectDoubleFactory();
                case JSonReaderNumberFormat.AsDecimal:
                    return new JSonMutableObjectDecimalFactory();
                default:
                    throw new ArgumentOutOfRangeException("format", string.Concat("Unsupported number reading format (", format, ")"));
            }
        }

        private JSonMutableObjectFactory()
        {
            Format = JSonReaderNumberFormat.Default;
        }

        protected JSonMutableObjectFactory(JSonReaderNumberFormat format)
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
            return new JSonMutableArray(data);
        }

        public object CreateObject(Dictionary<String, Object> data)
        {
            return new JSonMutableDictionary(data);
        }

        public object CreateKeyword(TokenDataString keyword)
        {
            return keyword.ValueAsJSonObject;
        }

        public object CreateString(string data)
        {
            return new JSonMutableStringObject(data);
        }

        public virtual object CreateNumber(string data)
        {
            return ObjectFactoryHelper.ParseNumber(this, data);
        }

        public object CreateNumber(Int32 data)
        {
            return new JSonMutableDecimalInt32Object(data);
        }

        public object CreateNumber(Int64 data)
        {
            return new JSonMutableDecimalInt64Object(data);
        }

        public object CreateNumber(UInt64 data)
        {
            return new JSonMutableDecimalUInt64Object(data);
        }

        public object CreateNumber(Double data)
        {
            return new JSonMutableDecimalDoubleObject(data);
        }

        public object CreateNumber(Decimal data)
        {
            return new JSonMutableDecimalDecimalObject(data);
        }
    }
}
