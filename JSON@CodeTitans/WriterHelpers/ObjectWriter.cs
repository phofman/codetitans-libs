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
    /// Helper class to write object begin/end statements.
    /// </summary>
    internal sealed class ObjectWriter : IJSonWriterObjectItem
    {
        private readonly IJSonWriter _output;

        public ObjectWriter(IJSonWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            _output = output;
            GC.SuppressFinalize(this);
        }

        internal ObjectWriter WriteObjectBegin()
        {
            _output.WriteObjectBegin();
            return this;
        }

        public void Dispose()
        {
            _output.WriteObjectEnd();
        }

        public void WriteMember(string name, DBNull value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, string value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, int value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, uint value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, long value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, ulong value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, float value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, double value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, decimal value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, DateTime value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, DateTime value, JSonWriterDateFormat format)
        {
            _output.WriteMember(name, value, format);
        }

        public void WriteMember(string name, TimeSpan value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, bool value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMember(string name, Guid value)
        {
            _output.WriteMember(name, value);
        }

        public void WriteMemberNull(string name)
        {
            _output.WriteMemberNull(name);
        }

        public void WriteMember(string name)
        {
            _output.WriteMember(name);
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
