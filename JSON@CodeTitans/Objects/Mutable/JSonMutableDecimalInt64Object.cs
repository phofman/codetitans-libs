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
using CodeTitans.Helpers;

namespace CodeTitans.JSon.Objects.Mutable
{
    internal sealed class JSonMutableDecimalInt64Object : JSonDecimalInt64Object, IJSonMutableObject
    {
        public JSonMutableDecimalInt64Object(long data)
            : base(data)
        {
        }

        public JSonMutableDecimalInt64Object(UInt64 data)
            : base(data)
        {
        }

        public JSonMutableDecimalInt64Object(long data, string stringRepresentation)
            : base(data, stringRepresentation)
        {
        }

        public JSonMutableDecimalInt64Object(DateTime date, JSonDateTimeKind kind)
            : base(date, kind)
        {
        }

        public JSonMutableDecimalInt64Object(TimeSpan time)
            : base(time)
        {
        }

        bool IJSonObject.IsMutable
        {
            get { return true; }
        }

        IJSonMutableObject IJSonObject.CreateMutableClone()
        {
            return this;
        }

        #region Implementation of IJSonMutableObject

        void IJSonMutableObject.SetValue(string value)
        {
            Data = Int64.Parse(value);
        }

        void IJSonMutableObject.SetValue(int value)
        {
            Data = value;
        }

        void IJSonMutableObject.SetValue(uint value)
        {
            Data = value;
        }

        void IJSonMutableObject.SetValue(long value)
        {
            Data = value;
        }

        void IJSonMutableObject.SetValue(ulong value)
        {
            Data = (Int64) value;
        }

        void IJSonMutableObject.SetValue(float value)
        {
            Data = (Int64) value;
        }

        void IJSonMutableObject.SetValue(double value)
        {
            Data = (Int64) value;
        }

        public void SetValue(decimal value)
        {
            Data = (Int64) value;
        }

        void IJSonMutableObject.SetValue(DateTime value)
        {
            Data = DateTimeHelper.ToNumber(value, JSonDateTimeKind.Default);
            StringRepresentation = JSonWriter.ToString(value);
        }

        public void SetValue(DateTime value, JSonDateTimeKind kind)
        {
            Data = DateTimeHelper.ToNumber(value, kind);
            StringRepresentation = JSonWriter.ToString(value);
        }

        void IJSonMutableObject.SetValue(TimeSpan value)
        {
            Data = value.Ticks;
            StringRepresentation = JSonWriter.ToString(value);
        }

        void IJSonMutableObject.SetValue(bool value)
        {
            Data = value ? 1L : 0L;
            StringRepresentation = JSonWriter.ToString(value);
        }

        void IJSonMutableObject.SetValue(Guid value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(IJSonObject value)
        {
            var input = value as JSonDecimalInt64Object;

            if (input != null)
            {
                Data = input.Data;
                StringRepresentation = input.StringRepresentation;
            }
            else
            {
                Data = value.Int64Value;
                StringRepresentation = null;
            }
        }

        void IJSonMutableObject.SetNull()
        {
            Data = 0;
            StringRepresentation = JSonReader.NullString;
        }

        void IJSonMutableObject.SetValueAt(int index, string value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, int value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, uint value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, long value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, ulong value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, float value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, double value)
        {
            throw new InvalidOperationException();
        }

        public void SetValueAt(int index, decimal value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, DateTime value)
        {
            throw new InvalidOperationException();
        }

        public void SetValueAt(int index, DateTime value, JSonDateTimeKind kind)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, TimeSpan value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, bool value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, Guid value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValueAt(int index, IJSonObject value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetNullAt(int index)
        {
            throw new InvalidOperationException();
        }

        IJSonMutableObject IJSonMutableObject.SetArrayAt(int index)
        {
            throw new InvalidOperationException();
        }

        IJSonMutableObject IJSonMutableObject.SetDictionaryAt(int index)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, string value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, int value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, uint value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, long value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, ulong value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, float value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, double value)
        {
            throw new InvalidOperationException();
        }

        public void InsertValueAt(int index, decimal value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, DateTime value)
        {
            throw new InvalidOperationException();
        }

        public void InsertValueAt(int index, DateTime value, JSonDateTimeKind kind)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, TimeSpan value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, bool value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, Guid value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertValueAt(int index, IJSonObject value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.InsertNullAt(int index)
        {
            throw new InvalidOperationException();
        }

        IJSonMutableObject IJSonMutableObject.InsertArrayAt(int index)
        {
            throw new InvalidOperationException();
        }

        IJSonMutableObject IJSonMutableObject.InsertDictionaryAt(int index)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.RemoveAt(int index)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.Clear()
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, string value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, int value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, uint value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, long value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, ulong value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, float value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, double value)
        {
            throw new InvalidOperationException();
        }

        public void SetValue(string name, decimal value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, DateTime value)
        {
            throw new InvalidOperationException();
        }

        public void SetValue(string name, DateTime value, JSonDateTimeKind kind)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, TimeSpan value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, bool value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, Guid value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(string name, IJSonObject value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetNull(string name)
        {
            throw new InvalidOperationException();
        }

        IJSonMutableObject IJSonMutableObject.SetArray(string name)
        {
            throw new InvalidOperationException();
        }

        IJSonMutableObject IJSonMutableObject.SetDictionary(string name)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.Remove(string name)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}