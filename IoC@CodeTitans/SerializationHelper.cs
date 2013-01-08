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

namespace CodeTitans.Services
{
#if WINDOWS_PHONE || SILVERLIGHT
    /// <summary>
    /// Since Windows Phone7 is based on Silverlight and doesn't support automatic serialization,
    /// here we need to define missing attributes to allow our code to compile (as it is also
    /// compiled against .NET 2.0+, CompactFramework 2.0+ and Mono 2.0+, where it is defined).
    /// </summary>
    public class Serializable : Attribute
    {
    }
#endif
}
