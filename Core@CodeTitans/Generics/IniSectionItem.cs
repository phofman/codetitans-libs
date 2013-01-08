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
using System.Globalization;
using System.IO;
using System.Text;

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Class describing single item within an INI file section.
    /// </summary>
#if !PocketPC
    [System.Diagnostics.DebuggerDisplay("IniSectionItem Name={Name}, Value={Value}")]
#endif
    public sealed class IniSectionItem
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public IniSectionItem(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public IniSectionItem(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public IniSectionItem(string name, string value, string comment)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
            Value = value;
            Comment = string.IsNullOrEmpty(comment) ? null : comment;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the value of this item.
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        #endregion

        #region Type Converters

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string StringValue
        {
            get { return Value; }
        }

        /// <summary>
        /// Gets a boolean value.
        /// </summary>
        public bool BooleanValue
        {
            get
            {
                if (string.Compare("true", Value, StringComparison.OrdinalIgnoreCase) == 0
                    || string.Compare("yes", Value, StringComparison.OrdinalIgnoreCase) == 0
                    || Value == "1")
                    return true;

                if (string.Compare("false", Value, StringComparison.OrdinalIgnoreCase) == 0
                    || string.Compare("no", Value, StringComparison.OrdinalIgnoreCase) == 0
                    || Value == "0")
                    return false;

                return Boolean.Parse(Value);
            }
        }

        /// <summary>
        /// Gets the Int32 value.
        /// </summary>
        public int Int32Value
        {
            get { return Int32.Parse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the UInt32 value.
        /// </summary>
        public uint UInt32Value
        {
            get { return UInt32.Parse(Value, NumberStyles.Integer, CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the Double value.
        /// </summary>
        public double DoubleValue
        {
            get { return Double.Parse(Value, NumberStyles.Float, CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the DateTime value.
        /// </summary>
        public DateTime DateTimeValue
        {
            get { return DateTime.Parse(Value, CultureInfo.InvariantCulture, DateTimeStyles.None); }
        }

        /// <summary>
        /// Gets the TimeSpan value.
        /// </summary>
        public TimeSpan TimeSpanValue
        {
            get { return TimeSpan.Parse(Value); }
        }

        #endregion

        internal void Write(StringBuilder output)
        {
            // serialize comment:
            IniSection.WriteComment(output, Comment);

            // serialize name and value:
            output.Append(Name).Append('=');
            if (!string.IsNullOrEmpty(Value))
            {
                if (char.IsWhiteSpace(Value, 0) || char.IsWhiteSpace(Value, Value.Length - 1))
                    output.Append('"').Append(Value).Append('"').Append(IniSection.NewLine);
                else
                    output.Append(Value).Append(IniSection.NewLine);
            }
            else
            {
                output.Append(IniSection.NewLine);
            }
        }

        internal void Write(TextWriter output)
        {
            // serialize comment:
            IniSection.WriteComment(output, Comment);

            // serialize name and value:
            output.Write(Name);
            output.Write('=');

            if (!string.IsNullOrEmpty(Value))
            {
                if (char.IsWhiteSpace(Value, 0) || char.IsWhiteSpace(Value, Value.Length - 1))
                {
                    output.Write('"');
                    output.Write(Value);
                    output.WriteLine('"');
                }
                else
                    output.WriteLine(Value);
            }
            else
            {
                output.WriteLine();
            }
        }

        /// <summary>
        /// Copies all data from another item.
        /// </summary>
        internal void CopyFrom(IniSectionItem item)
        {
            if (item != null)
            {
                Value = item.Value;
                Comment = item.Comment;
            }
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
