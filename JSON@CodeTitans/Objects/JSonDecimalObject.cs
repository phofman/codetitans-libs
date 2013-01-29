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
using CodeTitans.Helpers;

namespace CodeTitans.JSon.Objects
{
    /// <summary>
    /// Internal wrapper class that describes numeric type and provides <see cref="IJSonObject"/> access interface.
    /// </summary>
    internal abstract class JSonDecimalObject : IJSonObject
    {
        /// <summary>
        /// Gest the comparison epsilon value.
        /// </summary>
        protected const Double EpsilonDouble = 0.1d;
        /// <summary>
        /// Gets the comparison epsilon value.
        /// </summary>
        protected const Single EpsilonSingle = 0.1f;
        /// <summary>
        /// Gets the comparison epsilon value.
        /// </summary>
        protected static readonly Decimal EpsilonDecimal = new Decimal(EpsilonDouble);

        #region Protected IJSonObject Members

        protected abstract IJSonMutableObject GetMutableCopy();
        protected abstract IJSonObject GetImmutableCopy();
        protected abstract string GetStringValue();
        protected abstract int GetInt32Value();
        protected abstract uint GetUInt32Value();
        protected abstract long GetInt64Value();
        protected abstract ulong GetUInt64Value();
        protected abstract float GetSingleValue();
        protected abstract double GetDoubleValue();
        protected abstract decimal GetDecimalValue();
        protected abstract bool GetBooleanValue();
        protected abstract object GetObjectValue();

        #endregion

        #region IJSonObject Members

        string IJSonObject.StringValue
        {
            get { return GetStringValue(); }
        }

        int IJSonObject.Int32Value
        {
            get { return GetInt32Value(); }
        }

        uint IJSonObject.UInt32Value
        {
            get { return GetUInt32Value(); }
        }

        long IJSonObject.Int64Value
        {
            get { return GetInt64Value(); }
        }

        ulong IJSonObject.UInt64Value
        {
            get { return GetUInt64Value(); }
        }

        float IJSonObject.SingleValue
        {
            get { return GetSingleValue(); }
        }

        double IJSonObject.DoubleValue
        {
            get { return GetDoubleValue(); }
        }

        public decimal DecimalValue
        {
            get { return GetDecimalValue(); }
        }

        DateTime IJSonObject.DateTimeValue
        {
            get { return DateTimeHelper.ToDateTime(GetInt64Value(), JSonDateTimeKind.Default); }
        }

        TimeSpan IJSonObject.TimeSpanValue
        {
            get { return new TimeSpan(GetInt64Value()); }
        }

        bool IJSonObject.BooleanValue
        {
            get { return GetBooleanValue(); }
        }

        public bool IsArray
        {
            get { return false; }
        }

        object IJSonObject.ObjectValue
        {
            get { return GetObjectValue(); }
        }

        Guid IJSonObject.GuidValue
        {
            get { throw new InvalidOperationException(); }
        }

        bool IJSonObject.IsNull
        {
            get { return false; }
        }

        bool IJSonObject.IsTrue
        {
            get { return GetBooleanValue(); }
        }

        bool IJSonObject.IsFalse
        {
            get { return !GetBooleanValue(); }
        }

        bool IJSonObject.IsEnumerable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the DateTime value for given JSON object.
        /// </summary>
        public DateTime ToDateTimeValue(JSonDateTimeKind kind)
        {
            return DateTimeHelper.ToDateTime(GetInt64Value(), kind);
        }

        /// <summary>
        /// Gets the value of given JSON object.
        /// </summary>
        object IJSonObject.ToValue(Type t)
        {
            return JSonObjectConverter.ToObject(this, t);
        }

        /// <summary>
        /// Get the value of given JSON object.
        /// </summary>
        T IJSonObject.ToObjectValue<T>()
        {
            return (T)JSonObjectConverter.ToObject(this, typeof(T));
        }

        int IJSonObject.Length
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[int index]
        {
            get { throw new InvalidOperationException(); }
        }

        int IJSonObject.Count
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, IJSonObject defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, String defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, String defaultValue, Boolean asJSonSerializedObject]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Int32 defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name, uint defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Int64 defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name, ulong defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Single defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Double defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        public IJSonObject this[string name, decimal defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, DateTime defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[string name, DateTime defaultValue, JSonDateTimeKind kind]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, TimeSpan defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Guid defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IJSonObject IJSonObject.this[String name, Boolean defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        bool IJSonObject.Contains(string name)
        {
            return false;
        }

        ICollection<string> IJSonObject.Names
        {
            get { return null; }
        }

        IEnumerable<KeyValuePair<string, IJSonObject>> IJSonObject.ObjectItems
        {
            get { throw new InvalidOperationException(); }
        }

        bool IJSonObject.IsMutable
        {
            get { return false; }
        }

        IJSonMutableObject IJSonObject.AsMutable
        {
            get { return (IJSonMutableObject) this; }
        }

        IJSonMutableObject IJSonObject.CreateMutableClone()
        {
            return GetMutableCopy();
        }

        IJSonObject IJSonObject.CreateImmutableClone()
        {
            return GetImmutableCopy();
        }

        public string ToString(string format)
        {
            return GetStringValue();
        }

        IEnumerable<IJSonObject> IJSonObject.ArrayItems
        {
            get { throw new InvalidOperationException(); }
        }

        #endregion

        public override string ToString()
        {
            return GetStringValue();
        }
    }
}
