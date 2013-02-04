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

namespace CodeTitans.JSon
{
#if WINDOWS_PHONE || SILVERLIGHT || WINDOWS_STORE
    /// <summary>
    /// Since Windows Phone7 is based on Silverlight and doesn't support automatic serialization,
    /// here we need to define missing attributes to allow our code to compile (as it is also
    /// compiled against .NET 2.0+, CompactFramework 2.0+ and Mono 2.0+, where it is defined).
    /// </summary>
    public sealed class Serializable : Attribute
    {
    }
#endif

#if WINDOWS_STORE
    /// <summary>
    /// Since Windows Store enabled apps doesn't have access to System.Data namespace, to limit the number
    /// of code patching, let's define a missing static type.
    /// </summary>
    public sealed class DBNull
    {
        /// <summary>
        /// Global NUL value.
        /// </summary>
        public static readonly DBNull Value = new DBNull();
    }
#endif
}

namespace System.ComponentModel
{
#if WINDOWS_STORE
    /// <summary>
    /// Attribute to add description to any language element.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.All)]
    public sealed class DescriptionAttribute : Attribute
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public DescriptionAttribute(string description)
        {
            Description = description;
        }

        #region Properties

        /// <summary>
        /// Gets the description stored in this attribute.
        /// </summary>
        public string Description
        {
            get;
            private set;
        }

        #endregion
    }
#endif
}
