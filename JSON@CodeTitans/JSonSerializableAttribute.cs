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
using System.Reflection;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Attribute marking class or struct that support automatic serialization into and deserialization from JSON object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class JSonSerializableAttribute : Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonSerializableAttribute()
        {
            IgnoreStatic = true;
        }

#if !WINDOWS_STORE
        /// <summary>
        /// Gets binding flags used, when accessing fields and properties.
        /// </summary>
        internal BindingFlags Flags
        {
            get { return BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | (IgnoreStatic ? BindingFlags.Default : BindingFlags.Static); }
        }
#endif

        /// <summary>
        /// Gets or sets an indication if all fields should be used when serializing / deserializing, even if not marked with <see cref="JSonMemberAttribute"/>.
        /// </summary>
        public bool AllowAllFields
        { get; set; }

        /// <summary>
        /// Gets or sets an indication if all properties should be used when serializing / deserializing, even if not marked with <see cref="JSonMemberAttribute"/>.
        /// </summary>
        public bool AllowAllProperties
        { get; set; }

        /// <summary>
        /// Gets or sets an indication if static fields should be ignored when performing serialization / deserialization.
        /// </summary>
        public bool IgnoreStatic
        { get; set; }

        /// <summary>
        /// Gets or sets an indication if an <see cref="JSonMemberMissingException"/> should be thrown, when field or property is missing during deserialization.
        /// </summary>
        public bool SuppressThrowWhenMissing
        { get; set; }
    }
}
