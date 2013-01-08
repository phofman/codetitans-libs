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
    /// Info class about token.
    /// </summary>
    internal class TokenData
    {
        protected TokenData(JSonReaderTokenType type)
        {
            Type = type;
        }

        #region Propeties

        public JSonReaderTokenType Type
        { get; private set; }

        #endregion
    }
}
