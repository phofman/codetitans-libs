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
    /// Interface defining all serialization methods available for JSON Array.
    /// </summary>
    public interface IJSonWriterArrayItem : IJSonWriterItem
    {
        /// <summary>
        /// Writes a 'null' value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(DBNull value);

        /// <summary>
        /// Writes string value.
        /// It can be used only as an array element or value for object member.
        /// If 'null' value is passed it will emit the JSON 'null' for given member.
        /// </summary>
        void WriteValue(String value);

        /// <summary>
        /// Writes integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Int32 value);

        /// <summary>
        /// Writes unsigned integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(UInt32 value);

        /// <summary>
        /// Writes integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Int64 value);

        /// <summary>
        /// Writes unsigned long integer value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(UInt64 value);

        /// <summary>
        /// Writes decimal value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Single value);

        /// <summary>
        /// Writes decimal value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Double value);

        /// <summary>
        /// Writes decimal value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Decimal value);

        /// <summary>
        /// Writes DateTime value.
        /// It can be used only as an array element or value for object member.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        void WriteValue(DateTime value);

        /// <summary>
        /// Writes DateTime value.
        /// It can be used only as an array element or value for object member.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        void WriteValue(DateTime value, JSonWriterDateFormat format);

        /// <summary>
        /// Writes TimeSpan value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(TimeSpan value);

        /// <summary>
        /// Writes Boolean value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Boolean value);

        /// <summary>
        /// Writes Guid value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValue(Guid value);

        /// <summary>
        /// Writes 'null' value.
        /// It can be used only as an array element or value for object member.
        /// </summary>
        void WriteValueNull();

        /// <summary>
        /// Writes a value based on dynamic type checking.
        /// It can be then a string, number, embedded dictionary or array.
        /// </summary>
        void WriteValueDynamic(object value);
    }
}
