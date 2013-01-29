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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Interface implemented by any JSON object. It allows easy value extraction, conversion and type check.
    /// In case operation is not allowed on a given type (e.g.: enumeration of object over a decimal number)
    /// an <see cref="InvalidOperationException"/> will be thrown.
    /// </summary>
    public interface IJSonObject
    {
        /// <summary>
        /// Gets the string value.
        /// </summary>
        String StringValue
        { get; }

        /// <summary>
        /// Gets the Int32 value.
        /// </summary>
        Int32 Int32Value
        { get; }

        /// <summary>
        /// Gets the UInt32 value.
        /// </summary>
        UInt32 UInt32Value
        { get; }

        /// <summary>
        /// Gets the Int64 value.
        /// </summary>
        Int64 Int64Value
        { get; }

        /// <summary>
        /// Gets the UInt64 value.
        /// </summary>
        UInt64 UInt64Value
        { get; }

        /// <summary>
        /// Gets the Single value.
        /// </summary>
        Single SingleValue
        { get; }

        /// <summary>
        /// Gets the Double value.
        /// </summary>
        Double DoubleValue
        { get; }

        /// <summary>
        /// Gets the Decimal value.
        /// </summary>
        Decimal DecimalValue
        { get; }

        /// <summary>
        /// Gets the DateTime value.
        /// </summary>
        DateTime DateTimeValue
        { get; }

        /// <summary>
        /// Gets the TimeSpan value.
        /// </summary>
        TimeSpan TimeSpanValue
        { get; }

        /// <summary>
        /// Gets the Boolean value.
        /// </summary>
        Boolean BooleanValue
        { get; }

        /// <summary>
        /// Gets the Guid value.
        /// </summary>
        Guid GuidValue
        { get; }

        /// <summary>
        /// Checks if given value is null.
        /// </summary>
        Boolean IsNull
        { get; }

        /// <summary>
        /// Gets the Boolean value.
        /// </summary>
        Boolean IsTrue
        { get; }

        /// <summary>
        /// Gets the Boolean value.
        /// </summary>
        Boolean IsFalse
        { get; }

        /// <summary>
        /// Checks if given value is a collection.
        /// </summary>
        Boolean IsEnumerable
        { get; }

        /// <summary>
        /// Checks if given value is an array.
        /// </summary>
        Boolean IsArray
        { get; }

        /// <summary>
        /// Gets the raw, unwrapped copy of the value.
        /// </summary>
        Object ObjectValue
        { get; }

        /// <summary>
        /// Gets the DateTime value for given JSON object.
        /// </summary>
        DateTime ToDateTimeValue(JSonDateTimeKind kind);

        /// <summary>
        /// Gets the value of given JSON object.
        /// </summary>
        Object ToValue(Type type);

        /// <summary>
        /// Get the value of given JSON object.
        /// </summary>
        T ToObjectValue<T>();

        #region JSON Array Properties

        /// <summary>
        /// Gets the number of stored elements.
        /// </summary>
        Int32 Length
        { get; }

        /// <summary>
        /// Gets the element at given index.
        /// </summary>
        IJSonObject this[Int32 index]
        { get; }

        /// <summary>nonexistence
        /// Gets the enumerable collection of elements if this object was an array.
        /// </summary>
        IEnumerable<IJSonObject> ArrayItems
        { get; }

        #endregion

        #region JSON Object Properties

        /// <summary>
        /// Gets the number of stored elements.
        /// </summary>
        Int32 Count
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// </summary>
        IJSonObject this[String name]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, IJSonObject defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, String defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exist, the given string is converted as IJSonObject.
        /// Third parameter must be equal to 'true'.
        /// </summary>
        IJSonObject this[String name, String defaultValue, Boolean asJSonSerializedObject]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, Int32 defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, UInt32 defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, Int64 defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, UInt64 defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, Single defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, Double defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// If 
        /// </summary>
        IJSonObject this[String name, Decimal defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, DateTime defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, DateTime defaultValue, JSonDateTimeKind kind]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, TimeSpan defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, Guid defaultValue]
        { get; }

        /// <summary>
        /// Gets the member element with given name.
        /// In case of nonexistence, the default value will be returned.
        /// </summary>
        IJSonObject this[String name, Boolean defaultValue]
        { get; }

        /// <summary>
        /// Checks if member element with given name exists.
        /// </summary>
        bool Contains(String name);

        /// <summary>
        /// Gets the collection of names available inside given object.
        /// </summary>
        ICollection<String> Names
        { get; }

        /// <summary>
        /// Gets the enumerable collection of elements if this is an object.
        /// </summary>
        IEnumerable<KeyValuePair<String, IJSonObject>> ObjectItems
        { get; }

        #endregion

        #region Mutable JSON Object Extensions

        /// <summary>
        /// Gets an indication, if given object is mutable.
        /// </summary>
        bool IsMutable
        { get; }

        /// <summary>
        /// Gets the mutable version of this object if available or throws cast exception.
        /// </summary>
        IJSonMutableObject AsMutable
        { get; }

        /// <summary>
        /// Creates mutable clone of this object and all underlaying items (if available).
        /// </summary>
        IJSonMutableObject CreateMutableClone();

        /// <summary>
        /// Creates immutable clone of this object and all underlaying items (if available).
        /// </summary>
        IJSonObject CreateImmutableClone();

        #endregion

        /// <summary>
        /// Converts current IJSonObject into a string using formatting.
        /// Currently supported formats are:
        ///  - 'null' value - for using ToString() call
        ///  - 'i' - to force indented output
        ///  - 'n' - to disable indented output, but leave spaces after array item separators and between object name and value
        ///  - 'c' - to disable indented output and compact content as much as possible
        /// </summary>
        String ToString(String format);
    }
}
