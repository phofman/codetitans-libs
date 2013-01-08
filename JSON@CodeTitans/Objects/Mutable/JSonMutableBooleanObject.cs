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

namespace CodeTitans.JSon.Objects.Mutable
{
    internal sealed class JSonMutableBooleanObject : JSonBooleanObject, IJSonMutableObject
    {
        public JSonMutableBooleanObject(Boolean value)
            : base(value)
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
            if (value == "1")
                Data = true;
            else
            {
                if (value == "0")
                    Data = false;
                else
                {
                    Data = Boolean.Parse(value);
                }
            }
        }

        void IJSonMutableObject.SetValue(int value)
        {
            Data = value != 0;
        }

        void IJSonMutableObject.SetValue(uint value)
        {
            Data = value != 0;
        }

        void IJSonMutableObject.SetValue(long value)
        {
            Data = value != 0;
        }

        void IJSonMutableObject.SetValue(ulong value)
        {
            Data = value != 0;
        }

        void IJSonMutableObject.SetValue(float value)
        {
            Data = Math.Abs(value) > EpsilonSingle;
        }

        void IJSonMutableObject.SetValue(double value)
        {
            Data = Math.Abs(value) > EpsilonDouble;
        }

        public void SetValue(decimal value)
        {
            Data = Math.Abs(value) > EpsilonDecimal;
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
            Data = value != TimeSpan.Zero;
        }

        void IJSonMutableObject.SetValue(Boolean value)
        {
            Data = value;
        }

        void IJSonMutableObject.SetValue(Guid value)
        {
            Data = value != Guid.Empty;
        }

        void IJSonMutableObject.SetValue(IJSonObject value)
        {
            Data = value != null && value.BooleanValue;
        }

        void IJSonMutableObject.SetNull()
        {
            Data = false;
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