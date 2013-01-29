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
    /// Interface defining all types of writings that are possible to serialize object into a JSon string.
    /// </summary>
    public interface IJSonWriter : IJSonWriterArrayItem, IJSonWriterObjectItem
    {
        /// <summary>
        /// Checks if given instance of JSON writer contains a valid content.
        /// </summary>
        bool IsValid
        { get; }

        /// <summary>
        /// Writes an object start.
        /// </summary>
        void WriteObjectBegin();

        /// <summary>
        /// Writes an object end.
        /// </summary>
        void WriteObjectEnd();

        /// <summary>
        /// Writes an array start.
        /// </summary>
        void WriteArrayBegin();

        /// <summary>
        /// Writes an array end.
        /// </summary>
        void WriteArrayEnd();

        /// <summary>
        /// Closes the output and releases internal resources.
        /// </summary>
        void Close();
    }
}
