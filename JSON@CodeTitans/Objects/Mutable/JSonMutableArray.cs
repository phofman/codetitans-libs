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

namespace CodeTitans.JSon.Objects.Mutable
{
    internal sealed class JSonMutableArray : JSonArray, IJSonMutableObject
    {
        public JSonMutableArray(ICollection<object> data)
            : base(data, true)
        {
        }

        internal JSonMutableArray()
            : base(true)
        {
        }

        internal JSonMutableArray(JSonArray array)
            : base(true)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            foreach (var item in array.Data)
                Data.Add(item.CreateMutableClone());
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
            SetValue(Data.Count, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.SetValue(int value)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt32Object(value));
        }

        void IJSonMutableObject.SetValue(uint value)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt32Object((int)value));
        }

        void IJSonMutableObject.SetValue(long value)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValue(ulong value)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValue(float value)
        {
            SetValue(Data.Count, new JSonMutableDecimalSingleObject(value));
        }

        void IJSonMutableObject.SetValue(double value)
        {
            SetValue(Data.Count, new JSonMutableDecimalDoubleObject(value));
        }

        public void SetValue(decimal value)
        {
            SetValue(Data.Count, new JSonMutableDecimalDecimalObject(value));
        }

        void IJSonMutableObject.SetValue(DateTime value)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt64Object(value, JSonDateTimeKind.Default));
        }

        public void SetValue(DateTime value, JSonDateTimeKind kind)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt64Object(value, kind));
        }

        void IJSonMutableObject.SetValue(TimeSpan value)
        {
            SetValue(Data.Count, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValue(bool value)
        {
            SetValue(Data.Count, new JSonMutableBooleanObject(value));
        }

        void IJSonMutableObject.SetValue(Guid value)
        {
            SetValue(Data.Count, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.SetValue(IJSonObject value)
        {
            SetValue(Data.Count, value ?? new JSonMutableStringObject(null));
        }

        void IJSonMutableObject.SetNull()
        {
            Data.Clear();
        }

        private void SetValue(int index, IJSonObject value)
        {
            if (index >= Data.Count)
                Data.Add(value);
            else if (index < 0)
                Data.Insert(0, value);
            else
                Data[index] = value;
        }

        void IJSonMutableObject.SetValueAt(int index, string value)
        {
            SetValue(index, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.SetValueAt(int index, int value)
        {
            SetValue(index, new JSonMutableDecimalInt32Object(value));
        }

        void IJSonMutableObject.SetValueAt(int index, uint value)
        {
            SetValue(index, new JSonMutableDecimalInt32Object((int) value));
        }

        void IJSonMutableObject.SetValueAt(int index, long value)
        {
            SetValue(index, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValueAt(int index, ulong value)
        {
            SetValue(index, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValueAt(int index, float value)
        {
            SetValue(index, new JSonMutableDecimalSingleObject(value));
        }

        void IJSonMutableObject.SetValueAt(int index, double value)
        {
            SetValue(index, new JSonMutableDecimalDoubleObject(value));
        }

        public void SetValueAt(int index, decimal value)
        {
            SetValue(index, new JSonMutableDecimalDecimalObject(value));
        }

        void IJSonMutableObject.SetValueAt(int index, DateTime value)
        {
            SetValue(index, new JSonMutableDecimalInt64Object(value, JSonDateTimeKind.Default));
        }

        public void SetValueAt(int index, DateTime value, JSonDateTimeKind kind)
        {
            SetValue(index, new JSonMutableDecimalInt64Object(value, kind));
        }

        void IJSonMutableObject.SetValueAt(int index, TimeSpan value)
        {
            SetValue(index, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValueAt(int index, bool value)
        {
            SetValue(index, new JSonMutableBooleanObject(value));
        }

        void IJSonMutableObject.SetValueAt(int index, Guid value)
        {
            SetValue(index, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.SetValueAt(int index, IJSonObject value)
        {
            SetValue(index, value ?? new JSonMutableStringObject(null));
        }

        void IJSonMutableObject.SetNullAt(int index)
        {
            SetValue(index, new JSonMutableStringObject(null));
        }

        IJSonMutableObject IJSonMutableObject.SetArrayAt(int index)
        {
            IJSonMutableObject array = JSonMutableObject.CreateArray();

            SetValue(index, array);
            return array;
        }

        IJSonMutableObject IJSonMutableObject.SetDictionaryAt(int index)
        {
            IJSonMutableObject dict = JSonMutableObject.CreateDictionary();

            SetValue(index, dict);
            return dict;
        }

        private void InsertValue(int index, IJSonObject value)
        {
            if (index <= 0)
                Data.Insert(0, value);
            else
                if (index >= Data.Count)
                    Data.Add(value);
                else
                    Data.Insert(index, value);
        }

        void IJSonMutableObject.InsertValueAt(int index, string value)
        {
            InsertValue(index, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, int value)
        {
            InsertValue(index, new JSonMutableDecimalInt32Object(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, uint value)
        {
            InsertValue(index, new JSonMutableDecimalUInt32Object(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, long value)
        {
            InsertValue(index, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, ulong value)
        {
            InsertValue(index, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, float value)
        {
            InsertValue(index, new JSonMutableDecimalSingleObject(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, double value)
        {
            InsertValue(index, new JSonMutableDecimalDoubleObject(value));
        }

        public void InsertValueAt(int index, decimal value)
        {
            InsertValue(index, new JSonMutableDecimalDecimalObject(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, DateTime value)
        {
            InsertValue(index, new JSonMutableDecimalInt64Object(value, JSonDateTimeKind.Default));
        }

        public void InsertValueAt(int index, DateTime value, JSonDateTimeKind kind)
        {
            InsertValue(index, new JSonMutableDecimalInt64Object(value, kind));
        }

        void IJSonMutableObject.InsertValueAt(int index, TimeSpan value)
        {
            InsertValue(index, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, bool value)
        {
            InsertValue(index, new JSonMutableBooleanObject(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, Guid value)
        {
            InsertValue(index, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.InsertValueAt(int index, IJSonObject value)
        {
            InsertValue(index, value ?? new JSonMutableStringObject(null));
        }

        void IJSonMutableObject.InsertNullAt(int index)
        {
            InsertValue(index, new JSonMutableStringObject(null));
        }

        IJSonMutableObject IJSonMutableObject.InsertArrayAt(int index)
        {
            IJSonMutableObject array = JSonMutableObject.CreateArray();

            InsertValue(index, array);
            return array;
        }

        IJSonMutableObject IJSonMutableObject.InsertDictionaryAt(int index)
        {
            IJSonMutableObject dict = JSonMutableObject.CreateDictionary();

            InsertValue(index, dict);
            return dict;
        }

        void IJSonMutableObject.RemoveAt(int index)
        {
            Data.RemoveAt(index);
        }

        void IJSonMutableObject.Clear()
        {
            Data.Clear();
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

        public void SetValue(string name, IJSonObject value)
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