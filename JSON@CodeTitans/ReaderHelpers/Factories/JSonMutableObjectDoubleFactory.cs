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
    /// Object factory class providing numbers as Double type.
    /// </summary>
    internal sealed class JSonMutableObjectDoubleFactory : JSonMutableObjectFactory
    {
        internal JSonMutableObjectDoubleFactory()
            : base(JSonReaderNumberFormat.AsDouble)
        {
        }

        public override object CreateNumber(string data)
        {
            return ObjectFactoryHelper.ParseDouble(this, data);
        }
    }
}
