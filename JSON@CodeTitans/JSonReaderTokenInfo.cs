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
    internal class JSonReaderTokenInfo
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonReaderTokenInfo(string text, JSonReaderTokenType type, int line, int offset)
        {
            Text = text;
            Type = type;
            Line = line;
            Offset = offset;
        }

        #region Properties

        public string Text
        { get; private set; }

        public JSonReaderTokenType Type
        { get; private set; }

        public int Line
        { get; private set; }

        public int Offset
        { get; private set; }

        #endregion
    }
}
