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

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Interface providing access to Property List dictionary items.
    /// </summary>
    public interface IPropertyListDictionary
    {
        /// <summary>
        /// Gets the number of dictionary items.
        /// </summary>
        Int32 Count
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// </summary>
        IPropertyListItem this[String key]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        IPropertyListItem this[String key, Int32 defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        IPropertyListItem this[String key, String defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        IPropertyListItem this[String key, Double defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        IPropertyListItem this[String key, Byte[] defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        IPropertyListItem this[String key, DateTime defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        IPropertyListItem this[String key, Boolean defaultValue]
        { get; }

        /// <summary>
        /// Gets the collection of all internally stored key names (if dictionary).
        /// </summary>
        ICollection<String> Keys
        { get; }

        /// <summary>
        /// Checks if member element with given name exists.
        /// </summary>
        Boolean Contains(String key);

        /// <summary>
        /// Gets the enumerable collection of elements if this is a dictionary object.
        /// </summary>
        IEnumerable<KeyValuePair<String, IPropertyListItem>> DictionaryItems
        { get; }

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String key, Int32 value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String key, String value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String key, Double value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String key, Byte[] value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String key, DateTime value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String key, Boolean value);

        /// <summary>
        /// Adds new dictionary item.
        /// </summary>
        IPropertyListItem AddNewDictionary(String key);

        /// <summary>
        /// Adds new array.
        /// </summary>
        IPropertyListItem AddNewArray(String key);

        /// <summary>
        /// Removes item at given key.
        /// Returns that item or null if didn't exist.
        /// </summary>
        IPropertyListItem Remove(String key);

        /// <summary>
        /// Removes all items.
        /// </summary>
        void Clear();
    }
}
