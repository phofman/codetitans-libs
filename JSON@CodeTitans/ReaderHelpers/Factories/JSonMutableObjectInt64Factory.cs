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

namespace CodeTitans.JSon.ReaderHelpers.Factories
{
    /// <summary>
    /// Object factory class providing numbers as Int64 type.
    /// </summary>
    internal sealed class JSonMutableObjectInt64Factory : JSonMutableObjectFactory
    {
        internal JSonMutableObjectInt64Factory()
            : base(JSonReaderNumberFormat.AsInt64)
        {
        }

        public override object CreateNumber(string data)
        {
            return ObjectFactoryHelper.ParseInt64(this, data);
        }
    }
}
