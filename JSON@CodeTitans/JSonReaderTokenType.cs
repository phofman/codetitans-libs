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
    /// Enumeration describing possible JSON tokens.
    /// </summary>
    internal enum JSonReaderTokenType
    {
        Unknown,
        ObjectStart,
        ObjectEnd,
        ArrayStart,
        ArrayEnd,
        String,
        Number,
        Comma,
        Colon,
        Keyword,
        EndOfText
    }
}
