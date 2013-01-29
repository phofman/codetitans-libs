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

namespace CodeTitans.JSon.ReaderHelpers
{
    /// <summary>
    /// Class describing string tokens.
    /// </summary>
    internal sealed class TokenDataString : TokenData
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public TokenDataString(string token, JSonReaderTokenType type, object value, IJSonObject jValue)
            : base(type)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException("token");

            Token = token;
            Value = value;
            ValueAsJSonObject = jValue;
        }

        #region Properties

        public string Token
        { get; private set; }

        public object Value
        { get; private set; }

        public IJSonObject ValueAsJSonObject
        { get; private set; }

        #endregion
    }
}
