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
    /// Interface implemented by all types of custom classes that support JSON serialization.
    /// </summary>
    public interface IJSonWritable
    {
        /// <summary>
        /// Serializes an object as a JSON formatted string.
        /// </summary>
        void Write(IJSonWriter output);
    }

    /// <summary>
    /// Interface implemented by all types of custom classes that support JSON deserialization.
    /// </summary>
    public interface IJSonReadable
    {
        /// <summary>
        /// Deserialize an object from a JSON data input.
        /// </summary>
        void Read(IJSonObject input);
    }

    /// <summary>
    /// Interface implemented by custom classes supporting JSON serialization and deserialization.
    /// </summary>
    public interface IJSonSerializable : IJSonWritable, IJSonReadable
    {
    }
}
