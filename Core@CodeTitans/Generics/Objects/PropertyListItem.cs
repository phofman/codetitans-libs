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
using System.Collections.Generic;
using System.Globalization;

namespace CodeTitans.Core.Generics.Objects
{
    /// <summary>
    /// Internal class that encapsulates operations performed over a static value (like number/string/date).
    /// </summary>
    internal abstract class PropertyListItem : IPropertyListItem
    {
        private readonly string _key;
        private readonly PropertyListItemTypes _type;

        /// <summary>
        /// Hidden constructor.
        /// </summary>
        protected PropertyListItem(string key, PropertyListItemTypes type)
        {
            _key = key;
            _type = type;
        }

        #region Abstract Methods

        protected abstract Int32 GetInt32Value();
        protected abstract String GetStringValue();
        protected abstract Double GetDoubleValue();
        protected abstract DateTime GetDateTimeValue();
        protected abstract Boolean GetBooleanValue();

        #endregion

        #region IPropertyListItem Members

        string IPropertyListItem.Key
        {
            get { return _key; }
        }

        bool IPropertyListItem.IsEnumerable
        {
            get { return false; }
        }

        public bool IsArray
        {
            get { return false; }
        }

        int IPropertyListItem.Int32Value
        {
            get { return GetInt32Value(); }
        }

        string IPropertyListItem.StringValue
        {
            get { return GetStringValue(); }
        }

        double IPropertyListItem.DoubleValue
        {
            get { return GetDoubleValue(); }
        }

        byte[] IPropertyListItem.BinaryValue
        {
            get { throw new InvalidOperationException(); }
        }

        DateTime IPropertyListItem.DateTimeValue
        {
            get { return GetDateTimeValue(); }
        }

        bool IPropertyListItem.BooleanValue
        {
            get { return GetBooleanValue(); }
        }

        PropertyListItemTypes IPropertyListItem.Type
        {
            get { return _type; }
        }

        int IPropertyListDictionary.Count
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key]
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, int defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, string defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, double defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, byte[] defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, DateTime defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, bool defaultValue]
        {
            get { throw new InvalidOperationException(); }
        }

        ICollection<string> IPropertyListDictionary.Keys
        {
            get { throw new InvalidOperationException(); }
        }

        bool IPropertyListDictionary.Contains(string key)
        {
            return false;
        }

        IEnumerable<KeyValuePair<string, IPropertyListItem>> IPropertyListDictionary.DictionaryItems
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, int value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, string value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, double value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, byte[] value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, DateTime value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, bool value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.AddNewDictionary(string key)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.AddNewArray(string key)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Remove(String key)
        {
            throw new InvalidOperationException();
        }

        void IPropertyListDictionary.Clear()
        {
            throw new InvalidOperationException();
        }

        int IPropertyListItem.Length
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListItem.this[int index]
        {
            get { throw new InvalidOperationException(); }
        }

        IEnumerable<IPropertyListItem> IPropertyListItem.ArrayItems
        {
            get { throw new InvalidOperationException(); }
        }

        IPropertyListItem IPropertyListItem.Add(int value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.Add(string value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.Add(double value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.Add(byte[] value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.Add(DateTime value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.Add(bool value)
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.AddNewDictionary()
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.AddNewArray()
        {
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListItem.RemoveAt(Int32 index)
        {
            throw new InvalidOperationException();
        }

        #endregion

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_key))
                return GetStringValue();

            return string.Concat(_key, ": ", GetStringValue());
        }
    }

    internal sealed class PropertyListInt32Item : PropertyListItem
    {
        private readonly Int32 _data;

        public PropertyListInt32Item(string key, Int32 value)
            : base (key, PropertyListItemTypes.IntegerNumber)
        {
            _data = value;
        }

        protected override Int32 GetInt32Value()
        {
            return _data;
        }

        protected override String GetStringValue()
        {
            return _data.ToString(CultureInfo.InvariantCulture);
        }

        protected override Double GetDoubleValue()
        {
            return _data;
        }

        protected override DateTime GetDateTimeValue()
        {
            return new DateTime(_data, DateTimeKind.Utc);
        }

        protected override Boolean GetBooleanValue()
        {
            return _data != 0;
        }
    }

    internal sealed class PropertyListDoubleItem : PropertyListItem
    {
        private readonly Double _data;

        public PropertyListDoubleItem(string key, Double value)
            : base(key, PropertyListItemTypes.FloatingNumber)
        {
            _data = value;
        }

        protected override Int32 GetInt32Value()
        {
            return (int)_data;
        }

        protected override String GetStringValue()
        {
            return _data.ToString(CultureInfo.InvariantCulture);
        }

        protected override Double GetDoubleValue()
        {
            return _data;
        }

        protected override DateTime GetDateTimeValue()
        {
            return new DateTime((long)_data, DateTimeKind.Utc);
        }

        protected override Boolean GetBooleanValue()
        {
            return _data != 0;
        }
    }

    internal sealed class PropertyListStringItem : PropertyListItem
    {
        private readonly String _data;

        public PropertyListStringItem(string key, String value)
            : base(key, PropertyListItemTypes.String)
        {
            _data = value;
        }

        protected override Int32 GetInt32Value()
        {
            return Int32.Parse(_data, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        protected override String GetStringValue()
        {
            return _data;
        }

        protected override Double GetDoubleValue()
        {
            return Double.Parse(_data, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        protected override DateTime GetDateTimeValue()
        {
            return DateTime.Parse(_data, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        }

        protected override Boolean GetBooleanValue()
        {
            return Boolean.Parse(_data);
        }
    }

    internal sealed class PropertyListDateTimeItem : PropertyListItem
    {
        private readonly DateTime _data;

        public PropertyListDateTimeItem(string key, DateTime value)
            : base(key, PropertyListItemTypes.DateTime)
        {
            _data = value;
        }

        protected override Int32 GetInt32Value()
        {
            return (int)_data.ToUniversalTime().Ticks;
        }

        protected override String GetStringValue()
        {
            return _data.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture);
        }

        protected override Double GetDoubleValue()
        {
            return _data.ToUniversalTime().Ticks;
        }

        protected override DateTime GetDateTimeValue()
        {
            return _data;
        }

        protected override Boolean GetBooleanValue()
        {
            return _data != DateTime.MinValue && _data != DateTime.MaxValue;
        }
    }

    internal sealed class PropertyListBooleanItem : PropertyListItem
    {
        private readonly Boolean _data;

        public PropertyListBooleanItem(string key, Boolean value)
            : base(key, PropertyListItemTypes.Boolean)
        {
            _data = value;
        }

        protected override Int32 GetInt32Value()
        {
            return _data ? 1 : 0;
        }

        protected override String GetStringValue()
        {
            return _data ? Boolean.TrueString : Boolean.FalseString;
        }

        protected override Double GetDoubleValue()
        {
            return _data ? 1.0d : 0.0d;
        }

        protected override DateTime GetDateTimeValue()
        {
            return _data ? DateTime.MinValue : DateTime.MaxValue;
        }

        protected override Boolean GetBooleanValue()
        {
            return _data;
        }
    }

}
