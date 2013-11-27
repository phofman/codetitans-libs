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
    /// Interface defining all serialization methods available for JSON Object.
    /// </summary>
    public interface IJSonWriterObjectItem : IJSonWriterItem
    {
        /// <summary>
        /// Writes a 'null' text as a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, DBNull value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// If 'null' value is passed it will emit the JSON 'null' for given member.
        /// </summary>
        void WriteMember(string name, String value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Int32 value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, UInt32 value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Int64 value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, UInt64 value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Single value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Double value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Decimal value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        void WriteMember(string name, DateTime value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// Before writing, the date is converted into universal time representation and written as string.
        /// </summary>
        void WriteMember(string name, DateTime value, JSonWriterDateFormat format);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, TimeSpan value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Boolean value);

        /// <summary>
        /// Writes a member with a value.
        /// It requires to be put inside an object.
        /// </summary>
        void WriteMember(string name, Guid value);

        /// <summary>
        /// Writes 'null' JSON value.
        /// </summary>
        void WriteMemberNull(string name);

        /// <summary>
        /// Writes a member only to an object.
        /// It requires to be called before writing an object (or array) inside current object.
        /// Writing values after call to this function is also allowed, however it is not recommended
        /// due to number of checks performed. The better performance can be achieved
        /// if other overloaded functions are used.
        /// </summary>
        void WriteMember(string name);
    }
}
