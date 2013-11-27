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
using CodeTitans.JSon.Objects;
using System.Globalization;

namespace CodeTitans.JSon
{
    /// <summary>
    /// Attribute describing JSON object member created based on given field or property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class JSonMemberAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public JSonMemberAttribute()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, Single defaultValue)
        {
            Name = name;
            DefaultValue = new JSonDecimalSingleObject(defaultValue);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, Double defaultValue)
        {
            Name = name;
            DefaultValue = new JSonDecimalDoubleObject(defaultValue);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, DateTime defaultValue)
        {
            Name = name;
            DefaultValue = new JSonStringObject(defaultValue.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, TimeSpan defaultValue)
        {
            Name = name;
            DefaultValue = new JSonStringObject(defaultValue.ToString());
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, Boolean defaultValue)
        {
            Name = name;
            DefaultValue = new JSonBooleanObject(defaultValue);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, Guid defaultValue)
        {
            Name = name;
            DefaultValue = new JSonStringObject(defaultValue.ToString("D"));
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, Int64 defaultValue)
        {
            Name = name;
            DefaultValue = new JSonDecimalInt64Object(defaultValue);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, Int32 defaultValue)
        {
            Name = name;
            DefaultValue = new JSonDecimalInt32Object(defaultValue);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public JSonMemberAttribute(string name, string defaultValue)
        {
            Name = name;
            DefaultValue = new JSonStringObject(defaultValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets name of the JSON object's member.
        /// </summary>
        public string Name
        { get; private set; }

        /// <summary>
        /// Gets the default value when deserialized JSON misses that field.
        /// </summary>
        public IJSonObject DefaultValue
        { get; private set; }

        /// <summary>
        /// Gets or sets an indication that if value of this item is null, then it won't be serialized.
        /// </summary>
        public bool SkipWhenNull
        { get; set; }

        /// <summary>
        /// Gets or sets an indication if an <see cref="JSonMemberMissingException"/> should be thrown, when field or property is missing during deserialization.
        /// </summary>
        public bool SuppressThrowWhenMissing
        { get; set; }

        /// <summary>
        /// Gets or sets type that will override the field's / property's type.
        /// </summary>
        public Type ReadAs
        { get; set; }

        #endregion
    }
}
