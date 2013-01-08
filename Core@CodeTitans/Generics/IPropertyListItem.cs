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
    /// Interface providing access to Property List item.
    /// </summary>
    public interface IPropertyListItem : IPropertyListDictionary
    {
        /// <summary>
        /// Gets the key name of this item.
        /// </summary>
        String Key
        { get; }

        /// <summary>
        /// Gets an indication if given element is an array or dictionary.
        /// </summary>
        Boolean IsEnumerable
        { get; }

        /// <summary>
        /// Gets an indication if given element is an array.
        /// </summary>
        Boolean IsArray
        { get; }

        /// <summary>
        /// Gets the integer value.
        /// </summary>
        Int32 Int32Value
        { get; }

        /// <summary>
        /// Gets the String value.
        /// </summary>
        String StringValue
        { get; }

        /// <summary>
        /// Gets the floating point value.
        /// </summary>
        Double DoubleValue
        { get; }

        /// <summary>
        /// Gets the binary collection value.
        /// </summary>
        Byte[] BinaryValue
        { get; }

        /// <summary>
        /// Gets the DateTime value.
        /// </summary>
        DateTime DateTimeValue
        { get; }

        /// <summary>
        /// Gets the Boolean value.
        /// </summary>
        Boolean BooleanValue
        { get; }

        /// <summary>
        /// Gets the type of this item.
        /// </summary>
        PropertyListItemTypes Type
        { get; }

        #region PropertyListItem Array

        /// <summary>
        /// Gets the number of items stored within an array.
        /// </summary>
        Int32 Length
        { get; }

        /// <summary>
        /// Gets the element at given index.
        /// </summary>
        IPropertyListItem this[Int32 index]
        { get; }

        /// <summary>
        /// Gets the enumerable collection of elements if this object was an array.
        /// </summary>
        IEnumerable<IPropertyListItem> ArrayItems
        { get; }

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(Int32 value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(String value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(Double value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(Byte[] value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(DateTime value);

        /// <summary>
        /// Adds new item.
        /// </summary>
        IPropertyListItem Add(Boolean value);

        /// <summary>
        /// Adds new dictionary item.
        /// </summary>
        IPropertyListItem AddNewDictionary();

        /// <summary>
        /// Adds new array.
        /// </summary>
        IPropertyListItem AddNewArray();

        /// <summary>
        /// Removes an item at given index.
        /// Returns removed object or null in case index was invalid.
        /// </summary>
        IPropertyListItem RemoveAt(Int32 index);

        #endregion
    }
}
