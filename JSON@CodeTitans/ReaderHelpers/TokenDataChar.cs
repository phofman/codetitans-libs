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

namespace CodeTitans.JSon.ReaderHelpers
{
    /// <summary>
    /// Class describing single character tokens.
    /// </summary>
    internal sealed class TokenDataChar : TokenData
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public TokenDataChar(char token, JSonReaderTokenType type)
            : base(type)
        {
            Token = token;
        }

        #region Properties

        public char Token
        { get; private set; }

        #endregion
    }
}
