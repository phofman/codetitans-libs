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
    /// Internal class that encapsulates operations performed over <see cref="PropertyList"/> dictionaries.
    /// </summary>
    internal sealed class PropertyListDictionary : IPropertyListItem
    {
        private readonly string _key;
        private readonly Dictionary<string, IPropertyListItem> _items;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public PropertyListDictionary(string key)
        {
            _key = key;
            _items = new Dictionary<string, IPropertyListItem>();
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
            get { return false; }
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
            get { return PropertyListItemTypes.Dictionary; }
        }

        int IPropertyListDictionary.Count
        {
            get { return _items.Count; }
        }

        IPropertyListItem IPropertyListDictionary.this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                return _items[key];
            }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, int defaultValue]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                IPropertyListItem result;
                if (_items.TryGetValue(key, out result))
                    return result;

                return new PropertyListInt32Item(key, defaultValue);
            }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, string defaultValue]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                IPropertyListItem result;
                if (_items.TryGetValue(key, out result))
                    return result;

                return new PropertyListStringItem(key, defaultValue);
            }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, double defaultValue]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                IPropertyListItem result;
                if (_items.TryGetValue(key, out result))
                    return result;

                return new PropertyListDoubleItem(key, defaultValue);
            }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, byte[] defaultValue]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                IPropertyListItem result;
                if (_items.TryGetValue(key, out result))
                    return result;

                return new PropertyListBinary(key, defaultValue);
            }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, DateTime defaultValue]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                IPropertyListItem result;
                if (_items.TryGetValue(key, out result))
                    return result;

                return new PropertyListDateTimeItem(key, defaultValue);
            }
        }

        IPropertyListItem IPropertyListDictionary.this[string key, bool defaultValue]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                    throw new ArgumentNullException("key");

                IPropertyListItem result;
                if (_items.TryGetValue(key, out result))
                    return result;

                return new PropertyListBooleanItem(key, defaultValue);
            }
        }

        ICollection<string> IPropertyListDictionary.Keys
        {
            get { return _items.Keys; }
        }

        bool IPropertyListDictionary.Contains(string key)
        {
            return _items.ContainsKey(key);
        }

        IEnumerable<KeyValuePair<string, IPropertyListItem>> IPropertyListDictionary.DictionaryItems
        {
            get { return _items; }
        }

        private IPropertyListItem Add(IPropertyListItem item)
        {
            if (_items.ContainsKey(item.Key))
                _items[item.Key] = item;
            else
                _items.Add(item.Key, item);

            return item;
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, int value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListInt32Item(key, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListStringItem(key, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, double value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListDoubleItem(key, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, byte[] value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListBinary(key, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, DateTime value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListDateTimeItem(key, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.Add(string key, bool value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListBooleanItem(key, value);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.AddNewDictionary(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListDictionary(key);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.AddNewArray(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            var item = new PropertyListArray(key);
            return Add(item);
        }

        IPropertyListItem IPropertyListDictionary.Remove(String key)
        {
            IPropertyListItem item;

            if (_items.TryGetValue(key, out item))
            {
                _items.Remove(key);
                return item;
            }

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
            get { throw new InvalidOperationException(); }
        }

        IEnumerable<IPropertyListItem> IPropertyListItem.ArrayItems
        {
            get { return _items.Values; }
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
            var keys = _items.Keys;

            if (index >= 0 && index < keys.Count)
            {
                foreach (var key in keys)
                {
                    if (index == 0)
                    {
                        var item = _items[key];
                        _items.Remove(key);
                        return item;
                    }
                    index--;
                }
            }

            return null;
        }
    }
}
