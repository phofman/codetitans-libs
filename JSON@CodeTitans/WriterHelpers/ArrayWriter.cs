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

namespace CodeTitans.JSon.WriterHelpers
{
    /// <summary>
    /// Helper class to write array begin/end statements.
    /// </summary>
    internal sealed class ArrayWriter : IJSonWriterArrayItem
    {
        private readonly IJSonWriter _output;

        public ArrayWriter(IJSonWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            _output = output;
            GC.SuppressFinalize(this);
        }

        internal ArrayWriter WriteArrayBegin()
        {
            _output.WriteArrayBegin();
            return this;
        }

        public void Dispose()
        {
            _output.WriteArrayEnd();
        }

        public void WriteValue(DBNull value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(string value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(int value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(uint value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(long value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(ulong value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(float value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(double value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(decimal value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(DateTime value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(DateTime value, JSonWriterDateFormat format)
        {
            _output.WriteValue(value, format);
        }

        public void WriteValue(TimeSpan value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(bool value)
        {
            _output.WriteValue(value);
        }

        public void WriteValue(Guid value)
        {
            _output.WriteValue(value);
        }

        public void WriteValueNull()
        {
            _output.WriteValueNull();
        }

        public void WriteValueDynamic(object value)
        {
            _output.WriteValueDynamic(value);
        }

        public IJSonWriterObjectItem WriteObject()
        {
            return _output.WriteObject();
        }

        public IJSonWriterObjectItem WriteObject(string name)
        {
            return _output.WriteObject(name);
        }

        public IJSonWriterArrayItem WriteArray()
        {
            return _output.WriteArray();
        }

        public IJSonWriterArrayItem WriteArray(string name)
        {
            return _output.WriteArray(name);
        }

        public void Write(IEnumerable array)
        {
            _output.Write(array);
        }

        public void Write(IDictionary dictionary)
        {
            _output.Write(dictionary);
        }

        public void Write(IJSonWritable o)
        {
            _output.Write(o);
        }

        public void Write(object o)
        {
            _output.Write(o);
        }
    }
}
