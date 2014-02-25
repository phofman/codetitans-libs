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
using CodeTitans.JSon.WriterHelpers;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Binary JSON standard writer.
    /// Full specification can be found at: http://bsonspec.org
    /// </summary>
    public sealed class BSonWriter : IJSonWriter
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IJSonWriterObjectItem WriteObject()
        {
            return new ObjectWriter(this);
        }

        public IJSonWriterObjectItem WriteObject(string name)
        {
            WriteMember(name);
            return new ObjectWriter(this);
        }

        public IJSonWriterArrayItem WriteArray()
        {
            return new ArrayWriter(this);
        }

        public IJSonWriterArrayItem WriteArray(string name)
        {
            WriteMember(name);
            return new ArrayWriter(this);
        }

        public void Write(IEnumerable array)
        {
            throw new NotImplementedException();
        }

        public void Write(IDictionary dictionary)
        {
            throw new NotImplementedException();
        }

        public void Write(IJSonWritable o)
        {
            throw new NotImplementedException();
        }

        public void Write(object o)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(DBNull value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(string value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(int value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(uint value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(long value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(ulong value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(float value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(double value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(decimal value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(DateTime value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(DateTime value, JSonWriterDateFormat format)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(TimeSpan value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(bool value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(Guid value)
        {
            throw new NotImplementedException();
        }

        public void WriteValueNull()
        {
            throw new NotImplementedException();
        }

        public void WriteValueDynamic(object value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, DBNull value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, int value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, uint value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, long value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, ulong value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, float value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, double value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, decimal value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, DateTime value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, DateTime value, JSonWriterDateFormat format)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, TimeSpan value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, bool value)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name, Guid value)
        {
            throw new NotImplementedException();
        }

        public void WriteMemberNull(string name)
        {
            throw new NotImplementedException();
        }

        public void WriteMember(string name)
        {
            throw new NotImplementedException();
        }

        public bool IsValid
        {
            get;
            private set;
        }

        public void WriteObjectBegin()
        {
            throw new NotImplementedException();
        }

        public void WriteObjectEnd()
        {
            throw new NotImplementedException();
        }

        public void WriteArrayBegin()
        {
            throw new NotImplementedException();
        }

        public void WriteArrayEnd()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            Dispose();
        }

        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }
    }
}
