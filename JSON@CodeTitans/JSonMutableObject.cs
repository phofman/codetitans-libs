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

using CodeTitans.JSon.Objects;
using CodeTitans.JSon.Objects.Mutable;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Factory creating JSON mutable objects.
    /// </summary>
    public static class JSonMutableObject
    {
        /// <summary>
        /// Creates new instance of mutable JSON array object.
        /// </summary>
        public static IJSonMutableObject CreateArray()
        {
            return new JSonMutableArray();
        }

        /// <summary>
        /// Creates new instance of mutable JSON array object filled with given data.
        /// </summary>
        internal static IJSonMutableObject CreateArray(JSonArray array)
        {
            return new JSonMutableArray(array);
        }

        /// <summary>
        /// Creates new instance of mutable JSON dictionary object.
        /// </summary>
        public static IJSonMutableObject CreateDictionary()
        {
            return new JSonMutableDictionary();
        }

        /// <summary>
        /// Creates new instance of mutable JSON dictionary object filled with cloned data from source dictionary.
        /// </summary>
        internal static IJSonMutableObject CreateDictionary(JSonDictionary dict)
        {
            return new JSonMutableDictionary(dict);
        }
    }
}
