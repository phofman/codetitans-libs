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

using System.IO;

namespace CodeTitans.Helpers
{
    /// <summary>
    /// Helper class for managing string operations.
    /// </summary>
#if DEBUG && CODETITANS_LIB_CORE
    public
#else
    internal
#endif
    static class BinaryHelper
    {
        /// <summary>
        /// Creates a reader instance for a given buffer.
        /// </summary>
        public static IBinaryReader CreateReader(byte[] data)
        {
            return new ArrayReaderWrapper(data);
        }

        /// <summary>
        /// Creates a reader instance for a given stream.
        /// </summary>
        public static IBinaryReader CreateReader(BinaryReader data)
        {
            return null;
        }
    }
}
