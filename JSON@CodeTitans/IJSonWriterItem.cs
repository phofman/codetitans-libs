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

namespace CodeTitans.JSon
{
    /// <summary>
    /// Interface defining basic serialization methods available for both JSON Object and Array.
    /// </summary>
    public interface IJSonWriterItem : IDisposable
    {
        /// <summary>
        /// Writes automatically object begin/end statements, if call to this method is enclosed in 'using' statement.
        /// </summary>
        IJSonWriterObjectItem WriteObject();

        /// <summary>
        /// Writes automatically embedded object begin/end statements for parent object's member name, if call to this method is enclosed in 'using' statement.
        /// </summary>
        IJSonWriterObjectItem WriteObject(string name);

        /// <summary>
        /// Writes automatically array begin/end statements, if call to this method is enclosed in 'using' statement.
        /// </summary>
        IJSonWriterArrayItem WriteArray();

        /// <summary>
        /// Writes automatically embedded array begin/end statements for parent object's member name, if call to this method is enclosed in 'using' statement.
        /// </summary>
        IJSonWriterArrayItem WriteArray(string name);

        /// <summary>
        /// Writes enumerable collection as a JSON array.
        /// </summary>
        void Write(IEnumerable array);

        /// <summary>
        /// Writes a dictionary as a JSON object.
        /// </summary>
        void Write(IDictionary dictionary);

        /// <summary>
        /// Writes a serializable object as JSON string.
        /// </summary>
        void Write(IJSonWritable o);

        /// <summary>
        /// Writes whole object represented as dictionary or enumerable collection.
        /// </summary>
        void Write(Object o);
    }
}
