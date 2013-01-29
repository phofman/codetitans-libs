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

namespace CodeTitans.Core.Generics.Objects
{
    /// <summary>
    /// Internal class that encapsulates operations performed over array of <see cref="PropertyList"/> items.
    /// </summary>
    internal sealed class PropertyListArray : IPropertyListItem
    {
        private readonly string _key;
        private readonly List<IPropertyListItem> _items;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public PropertyListArray(string key)
        {
            _key = key;
            _items = new List<IPropertyListItem>();
        }

        string IPropertyListItem.Key
        {
            get { return _key; }
        }

        bool IPropertyListItem.IsEnumerable
        {
            get { return true; }
        }

        public bool IsArray
        {
            get { return true; }
        }

        int IPropertyListItem.Int32Value
        {
            get { throw new InvalidOperationException(); }
        }

        string IPropertyListItem.StringValue
        {
            get { throw new InvalidOperationException(); }
        }

        double IPropertyListItem.DoubleValue
        {
            get { throw new InvalidOperationException(); }
        }

        byte[] IPropertyListItem.BinaryValue
        {
            get { throw new InvalidOperationException(); }
        }

        DateTime IPropertyListItem.DateTimeValue
        {
            get { throw new InvalidOperationException(); }
        }

        bool IPropertyListItem.BooleanValue
        {
            get { throw new InvalidOperationException(); }
        }

        PropertyListItemTypes IPropertyListItem.Type
        {
            get { return PropertyListItemTypes.Array; }
        }

        int IPropertyListDictionary.Count
        {
            get { return _items.Count; }
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
            get { return null; }
        }

        bool IPropertyListDictionary.Contains(string key)
        {
            return false;
        }

        IEnumerable<KeyValuePair<string, IPropertyListItem>> IPropertyListDictionary.DictionaryItems
        {
            get { return null; }
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, int value)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).Add(value);
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).Add(value);
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, double value)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).Add(value);
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, byte[] value)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).Add(value);
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, DateTime value)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).Add(value);
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, bool value)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).Add(value);
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.AddNewDictionary(string key)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).AddNewDictionary();
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.AddNewArray(string key)
        {
            if (string.IsNullOrEmpty(key))
                return ((IPropertyListItem)this).AddNewArray();
            throw new InvalidOperationException();
        }

        IPropertyListItem IPropertyListDictionary.Remove(String key)
        {
            return null;
        }

        void IPropertyListDictionary.Clear()
        {
            _items.Clear();
        }

        int IPropertyListItem.Length
        {
            get { return _items.Count; }
        }

        IPropertyListItem IPropertyListItem.this[int index]
        {
            get { return _items[index]; }
        }

        IEnumerable<IPropertyListItem> IPropertyListItem.ArrayItems
        {
            get { return _items; }
        }

        private IPropertyListItem Add(IPropertyListItem item)
        {
            _items.Add(item);
            return item;
        }

        IPropertyListItem IPropertyListItem.Add(int value)
        {
            var item = new PropertyListInt32Item(null, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.Add(string value)
        {
            var item = new PropertyListStringItem(null, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.Add(double value)
        {
            var item = new PropertyListDoubleItem(null, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.Add(byte[] value)
        {
            var item = new PropertyListBinary(null, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.Add(DateTime value)
        {
            var item = new PropertyListDateTimeItem(null, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.Add(bool value)
        {
            var item = new PropertyListBooleanItem(null, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.AddNewDictionary()
        {
            var item = new PropertyListDictionary(null);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.AddNewArray()
        {
            var item = new PropertyListArray(null);
            return Add(item);
        }

        IPropertyListItem IPropertyListItem.RemoveAt(Int32 index)
        {
            if (index >= 0 && index < _items.Count)
            {
                var item = _items[index];
                _items.RemoveAt(index);
                return item;
            }

            return null;
        }
    }
}
