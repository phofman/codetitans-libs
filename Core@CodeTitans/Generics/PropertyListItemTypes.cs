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

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Enumeration describing types of Property List items.
    /// </summary>
    public enum PropertyListItemTypes
    {
        /// <summary>
        /// An item is an array.
        /// </summary>
        Array,
        /// <summary>
        /// An item is a dictionary.
        /// </summary>
        Dictionary,
        /// <summary>
        /// An item is a string.
        /// </summary>
        String,
        /// <summary>
        /// An item is a binary data.
        /// </summary>
        Binary,
        /// <summary>
        /// An item is a date.
        /// </summary>
        DateTime,
        /// <summary>
        /// An item is a number (System.Int32).
        /// </summary>
        IntegerNumber,
        /// <summary>
        /// An item is a floating point number (System.Double).
        /// </summary>
        FloatingNumber,
        /// <summary>
        /// An item is a logical value (System.Boolean).
        /// </summary>
        Boolean
    }
}
