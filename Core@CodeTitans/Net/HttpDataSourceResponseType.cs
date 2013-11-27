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

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Enumeration describing what type of response is expected from given IHttpDataSource.
    /// </summary>
    public enum HttpDataSourceResponseType
    {
        /// <summary>
        /// Should provide string response.
        /// </summary>
        AsString,
        /// <summary>
        /// Should provide binary response.
        /// </summary>
        AsBinary,
        /// <summary>
        /// Should provide access to underlying stream object if possible.
        /// </summary>
        AsRawStream
    }
}
