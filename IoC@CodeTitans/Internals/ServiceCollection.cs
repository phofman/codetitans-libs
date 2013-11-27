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

using System.Collections.Generic;

namespace CodeTitans.Services.Internals
{
    /// <summary>
    /// Synchronized access collection.
    /// </summary>
    internal sealed class ServiceCollection<T> : Dictionary<T,ServiceObjectWrapper>
    {
    }
}
