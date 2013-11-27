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
    /// Internal class that encapsulates the operations over binary data.
    /// </summary>
    internal sealed class PropertyListBinary : IPropertyListItem
    {
        private readonly string _key;
        private readonly byte[] _data;

        public PropertyListBinary(string key, byte[] data)
        {
            _key = key;
            _data = data;
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
            get { return Convert.ToBase64String(_data); }
        }

        double IPropertyListItem.DoubleValue
        {
            get { throw new InvalidOperationException(); }
        }

        byte[] IPropertyListItem.BinaryValue
        {
            get { return _data; }
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
            get { return PropertyListItemTypes.Binary; }
        }

        int IPropertyListDictionary.Count
        {
            get
            {
                if (_data != null)
                    return _data.Length;

                return 0;
            }
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
            get
            {
                if (_data != null)
                    return _data.Length;

                return 0;
            }
        }

        IPropertyListItem IPropertyListItem.this[int index]
        {
            get { return null; }
        }

        IEnumerable<IPropertyListItem> IPropertyListItem.ArrayItems
        {
            get { return null; }
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
    }
}
