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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Interface defining all read operation that might convert a JSON string into its tree representation that could be traversed.
    /// </summary>
    public interface IJSonReader
    {
        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        object Read();

        /// <summary>
        /// Converts a JSON string from given input into a tree of .NET arrays, dictionaries, strings and decimals.
        /// </summary>
        object Read(JSonReaderNumberFormat format);

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        IJSonObject ReadAsJSonObject();

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        IJSonObject ReadAsJSonObject(JSonReaderNumberFormat format);

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        IJSonMutableObject ReadAsJSonMutableObject();

        /// <summary>
        /// Converts a JSON string from given input into a tree of JSON-specific objects.
        /// It then allows easier deserialization for objects implementing <see cref="IJSonMutableObject"/> interface as those objects expose
        /// more functionality then the standard .NET ones.
        /// </summary>
        IJSonMutableObject ReadAsJSonMutableObject(JSonReaderNumberFormat format);
    }
}
