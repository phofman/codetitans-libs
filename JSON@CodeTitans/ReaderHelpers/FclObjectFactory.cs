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
using CodeTitans.JSon.ReaderHelpers.Factories;

namespace CodeTitans.JSon.ReaderHelpers
{
    /// <summary>
    /// Class that wraps operation on creating default .NET types based on read data.
    /// </summary>
    internal class FclObjectFactory : IObjectFactory
    {
        /// <summary>
        /// Creates a proper FCL factory for given numer formatting parsing.
        /// </summary>
        internal static IObjectFactory Create(JSonReaderNumberFormat format)
        {
            switch(format)
            {
                case JSonReaderNumberFormat.Default:
                    return new FclObjectFactory();
                case JSonReaderNumberFormat.AsInt32:
                    return new FclObjectInt32Factory();
                case JSonReaderNumberFormat.AsInt64:
                    return new FclObjectInt64Factory();
                case JSonReaderNumberFormat.AsDouble:
                    return new FclObjectDoubleFactory();
                case JSonReaderNumberFormat.AsDecimal:
                    return new FclObjectDecimalFactory();
                default:
                    throw new ArgumentOutOfRangeException("format", string.Concat("Unsupported number reading format (", format, ")"));
            }
        }

        private FclObjectFactory()
        {
            Format = JSonReaderNumberFormat.Default;
        }

        protected FclObjectFactory(JSonReaderNumberFormat format)
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
            return data.ToArray();
        }

        public object CreateObject(Dictionary<String, Object> data)
        {
            return data;
        }

        public object CreateKeyword(TokenDataString keyword)
        {
            return keyword.Value;
        }

        public object CreateString(String data)
        {
            return data;
        }

        public virtual object CreateNumber(string data)
        {
            return ObjectFactoryHelper.ParseNumber(this, data);
        }

        public object CreateNumber(Int32 data)
        {
            return data;
        }

        public object CreateNumber(Int64 data)
        {
            return data;
        }

        public object CreateNumber(UInt64 data)
        {
            return data;
        }

        public object CreateNumber(Double data)
        {
            return data;
        }

        public object CreateNumber(Decimal data)
        {
            return data;
        }
    }
}
