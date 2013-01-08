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
    internal sealed class JSonMutableDictionary : JSonDictionary, IJSonMutableObject
    {
        public JSonMutableDictionary(Dictionary<string, object> data)
            : base(data)
        {
        }

        internal JSonMutableDictionary()
            : base(true)
        {
        }

        internal JSonMutableDictionary(JSonDictionary dictionary)
            : base(true)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            foreach (KeyValuePair<string, IJSonObject> item in dictionary.Data)
                Data.Add(item.Key, item.Value.CreateMutableClone());
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
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(int value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(uint value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(long value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(ulong value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(float value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(double value)
        {
            throw new InvalidOperationException();
        }

        public void SetValue(decimal value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(DateTime value)
        {
            throw new InvalidOperationException();
        }

        public void SetValue(DateTime value, JSonDateTimeKind kind)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(TimeSpan value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(bool value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(Guid value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetValue(IJSonObject value)
        {
            throw new InvalidOperationException();
        }

        void IJSonMutableObject.SetNull()
        {
            Data.Clear();
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
            Data.Clear();
        }

        private void SetValue(string name, IJSonObject value)
        {
            if (Data.ContainsKey(name))
                Data[name] = value;
            else
                Data.Add(name, value);
        }

        void IJSonMutableObject.SetValue(string name, string value)
        {
            SetValue(name, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.SetValue(string name, int value)
        {
            SetValue(name, new JSonMutableDecimalInt32Object(value));
        }

        void IJSonMutableObject.SetValue(string name, uint value)
        {
            SetValue(name, new JSonMutableDecimalUInt32Object(value));
        }

        void IJSonMutableObject.SetValue(string name, long value)
        {
            SetValue(name, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValue(string name, ulong value)
        {
            SetValue(name, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValue(string name, float value)
        {
            SetValue(name, new JSonMutableDecimalSingleObject(value));
        }

        void IJSonMutableObject.SetValue(string name, double value)
        {
            SetValue(name, new JSonMutableDecimalDoubleObject(value));
        }

        public void SetValue(string name, decimal value)
        {
            SetValue(name, new JSonMutableDecimalDecimalObject(value));
        }

        void IJSonMutableObject.SetValue(string name, DateTime value)
        {
            SetValue(name, new JSonMutableDecimalInt64Object(value, JSonDateTimeKind.Default));
        }

        public void SetValue(string name, DateTime value, JSonDateTimeKind kind)
        {
            SetValue(name, new JSonMutableDecimalInt64Object(value, kind));
        }

        void IJSonMutableObject.SetValue(string name, TimeSpan value)
        {
            SetValue(name, new JSonMutableDecimalInt64Object(value));
        }

        void IJSonMutableObject.SetValue(string name, bool value)
        {
            SetValue(name, new JSonMutableBooleanObject(value));
        }

        void IJSonMutableObject.SetValue(string name, Guid value)
        {
            SetValue(name, new JSonMutableStringObject(value));
        }

        void IJSonMutableObject.SetValue(string name, IJSonObject value)
        {
            SetValue(name, value ?? new JSonMutableStringObject(null));
        }

        void IJSonMutableObject.SetNull(string name)
        {
            SetValue(name, new JSonMutableStringObject(null));
        }

        IJSonMutableObject IJSonMutableObject.SetArray(string name)
        {
            IJSonMutableObject array = JSonMutableObject.CreateArray();

            SetValue(name, array);
            return array;
        }

        IJSonMutableObject IJSonMutableObject.SetDictionary(string name)
        {
            IJSonMutableObject dict = JSonMutableObject.CreateDictionary();

            SetValue(name, dict);
            return dict;
        }

        void IJSonMutableObject.Remove(string name)
        {
            Data.Remove(name);
        }

        #endregion
    }
}