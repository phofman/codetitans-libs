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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Interface of mutable JSON object.
    /// </summary>
    public interface IJSonMutableObject : IJSonObject
    {
        /// <summary>
        /// Sets the string value.
        /// </summary>
        void SetValue(String value);

        /// <summary>
        /// Sets the Int32 value.
        /// </summary>
        void SetValue(Int32 value);

        /// <summary>
        /// Sets the UInt32 value.
        /// </summary>
        void SetValue(UInt32 value);

        /// <summary>
        /// Sets the Int64 value.
        /// </summary>
        void SetValue(Int64 value);

        /// <summary>
        /// Sets the UInt64 value.
        /// </summary>
        void SetValue(UInt64 value);

        /// <summary>
        /// Sets the float value.
        /// </summary>
        void SetValue(Single value);

        /// <summary>
        /// Sets the double value.
        /// </summary>
        void SetValue(Double value);

        /// <summary>
        /// Sets the decimal value.
        /// </summary>
        void SetValue(Decimal value);

        /// <summary>
        /// Sets the DateTime value.
        /// </summary>
        void SetValue(DateTime value);

        /// <summary>
        /// Sets the DateTime value.
        /// </summary>
        void SetValue(DateTime value, JSonDateTimeKind kind);

        /// <summary>
        /// Sets the TimeSpan value.
        /// </summary>
        void SetValue(TimeSpan value);

        /// <summary>
        /// Sets the bool value.
        /// </summary>
        void SetValue(Boolean value);

        /// <summary>
        /// Sets the Guid value.
        /// </summary>
        void SetValue(Guid value);

        /// <summary>
        /// Sets the IJSonObject value.
        /// </summary>
        void SetValue(IJSonObject value);

        /// <summary>
        /// Sets the null value or clears the collection.
        /// </summary>
        void SetNull();

        #region JSON Array Operations

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, String value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Int32 value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, UInt32 value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Int64 value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, UInt64 value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Single value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Double value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Decimal value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, DateTime value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, DateTime value, JSonDateTimeKind kind);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, TimeSpan value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Boolean value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, Guid value);

        /// <summary>
        /// Sets the element value at given index.
        /// </summary>
        void SetValueAt(Int32 index, IJSonObject value);

        /// <summary>
        /// Sets the null value at given index.
        /// </summary>
        void SetNullAt(Int32 index);

        /// <summary>
        /// Creates new mutable array at given index.
        /// </summary>
        IJSonMutableObject SetArrayAt(Int32 index);

        /// <summary>
        /// Creates new mutable dictionary at given index.
        /// </summary>
        IJSonMutableObject SetDictionaryAt(Int32 index);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, String value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Int32 value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, UInt32 value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Int64 value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, UInt64 value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Single value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Double value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Decimal value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, DateTime value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, DateTime value, JSonDateTimeKind kind);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, TimeSpan value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Boolean value);

        /// <summary>
        /// Inserts new value at given index.
        /// </summary>
        void InsertValueAt(Int32 index, Guid value);

        /// <summary>
        /// Inserts new values at given indx.
        /// </summary>
        void InsertValueAt(Int32 index, IJSonObject value);

        /// <summary>
        /// Inserts null value at given index.
        /// </summary>
        void InsertNullAt(Int32 index);

        /// <summary>
        /// Inserts new mutable array at given index.
        /// </summary>
        IJSonMutableObject InsertArrayAt(Int32 index);

        /// <summary>
        /// Inserts new mutable dictionary at given index.
        /// </summary>
        IJSonMutableObject InsertDictionaryAt(Int32 index);

        /// <summary>
        /// Removes item at given index.
        /// </summary>
        void RemoveAt(Int32 index);

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        void Clear();

        #endregion

        #region JSON Object Operations

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, String value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Int32 value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, UInt32 value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Int64 value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, UInt64 value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Single value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Double value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Decimal value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, DateTime value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, DateTime value, JSonDateTimeKind kind);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, TimeSpan value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Boolean value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, Guid value);

        /// <summary>
        /// Sets value with given name.
        /// </summary>
        void SetValue(String name, IJSonObject value);

        /// <summary>
        /// Sets null value with given name.
        /// </summary>
        void SetNull(String name);

        /// <summary>
        /// Adds new mutable array with given name.
        /// </summary>
        IJSonMutableObject SetArray(String name);

        /// <summary>
        /// Adds new mutable dictionary with given name.
        /// </summary>
        IJSonMutableObject SetDictionary(String name);

        /// <summary>
        /// Removes item with given name.
        /// </summary>
        void Remove(String name);

        #endregion
    }
}