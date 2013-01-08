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
    /// Enumeration describing JSON serialization tokens.
    /// </summary>
    public enum JSonWriterTokenType
    {
        /// <summary>
        /// No token.
        /// </summary>
        Nothing,
        /// <summary>
        /// Token is a JSON object.
        /// </summary>
        Object,
        /// <summary>
        /// Token is a JSON array.
        /// </summary>
        Array,
        /// <summary>
        /// Token is a JSON object member.
        /// </summary>
        MemberValue
    }
}
