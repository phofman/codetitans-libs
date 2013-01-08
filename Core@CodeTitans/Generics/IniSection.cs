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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Class describing single section within INI file.
    /// </summary>
#if !PocketPC
    [System.Diagnostics.DebuggerDisplay("IniSection Name={Name}, Count={Count}")]
#endif
    public sealed class IniSection : IEnumerable<IniSectionItem>
    {
        internal const char CommentChar = ';';
        internal const char CommentCharAlt = '#';
        internal const char SectionStart = '[';
        internal const char SectionEnd = ']';
#if PocketPC
        internal const string NewLine = "\r\n";
#else
        internal static readonly string NewLine = Environment.NewLine;
#endif
        private static readonly char[] CommentSeparators = new[] { '\r', '\n' };

        private readonly List<IniSectionItem> _items;
        private readonly IDictionary<string, IniSectionItem> _itemDir;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IniSection()
        {
            _items = new List<IniSectionItem>();
            _itemDir = new Dictionary<string, IniSectionItem>();
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public IniSection(string name)
        {
            Name = name;
            _items = new List<IniSectionItem>();
            _itemDir = new Dictionary<string, IniSectionItem>();
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public IniSection(string name, string comment)
        {
            Name = name;
            Comment = string.IsNullOrEmpty(comment) ? null : comment;
            _items = new List<IniSectionItem>();
            _itemDir = new Dictionary<string, IniSectionItem>();
        }

        #region Properties

        /// <summary>
        /// Gets or set the comment associated with this section.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of stored items.
        /// </summary>
        public IniSectionItem[] Items
        {
            get { return _items.ToArray(); }
        }

        /// <summary>
        /// Gets the item of specified name or null.
        /// Name is case insensitive.
        /// </summary>
        public IniSectionItem this[string name]
        {
            get
            {
                IniSectionItem item;
                return TryGetValue(name, out item) ? item : null;
            }
        }

        /// <summary>
        /// Gets the item of specified name.
        /// Name is case sensitive. If the item doesn't exist,
        /// it will create and return the temporary item with specified default value.
        /// </summary>
        public IniSectionItem this[string name, string defaultValue]
        {
            get
            {
                IniSectionItem item;
                return TryGetValue(name, out item) ? item : new IniSectionItem(name, defaultValue);
            }
        }

        /// <summary>
        /// Gets the number of stored items.
        /// </summary>
        public int Count
        {
            get { return _items.Count; }
        }

        #endregion

        /// <summary>
        /// Adds an item into the section.
        /// </summary>
        public void Add(IniSectionItem item)
        {
            if (item != null)
            {
                IniSectionItem existingItem;
                string lowerName = item.Name.ToLower();

                if (_itemDir.TryGetValue(lowerName, out existingItem))
                {
                    existingItem.CopyFrom(item);
                }
                else
                {
                    _items.Add(item);
                    _itemDir.Add(lowerName, item);
                }
            }
        }

        /// <summary>
        /// Adds new item into the section.
        /// </summary>
        public void Add(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Add(new IniSectionItem(name, value, null));
        }

        /// <summary>
        /// Retrieves the item from the section.
        /// Name is case insensitive.
        /// </summary>
        public bool TryGetValue(string name, out IniSectionItem value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            return _itemDir.TryGetValue(name.ToLower(), out value);
        }

        /// <summary>
        /// Checks if item with given name belongs to the section.
        /// </summary>
        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            return _itemDir.ContainsKey(name.ToLower());
        }

        /// <summary>
        /// Removes an item from section.
        /// </summary>
        public bool Remove(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            IniSectionItem existingItem;
            string lowerName = name.ToLower();

            if (_itemDir.TryGetValue(lowerName, out existingItem))
            {
                _itemDir.Remove(lowerName);
                _items.Remove(existingItem);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all items.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _itemDir.Clear();
        }

        /// <summary>
        /// Overwrites items in current section with new ones.
        /// </summary>
        internal void OverwriteWith(IniSection section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            foreach (var item in section)
            {
                Add(item);
            }
        }

        #region IEnumerator implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<IniSectionItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        internal void Write(StringBuilder output)
        {
            // serialize comment:
            WriteComment(output, Comment);

            // serialize name of the section:
            if (!string.IsNullOrEmpty(Name))
            {
                output.Append(SectionStart).Append(Name).Append(SectionEnd);
                output.Append(NewLine);
            }

            // serialize items:
            foreach (var item in _items)
                item.Write(output);
        }

        internal void Write(TextWriter output)
        {
            // serialize comment:
            WriteComment(output, Comment);

            // serialize name of the section:
            if (!string.IsNullOrEmpty(Name))
            {
                output.Write(SectionStart);
                output.Write(Name);
                output.Write(SectionEnd);
                output.WriteLine();
            }

            // serialize items:
            foreach (var item in _items)
                item.Write(output);
        }

        /// <summary>
        /// Serialize multiline comment.
        /// </summary>
        internal static void WriteComment(StringBuilder output, string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return;

            int index = comment.IndexOfAny(CommentSeparators);
            if (index >= 0)
            {
                // serialize multiline comment, line by line:
                int start = 0;
                while (index < comment.Length)
                {
                    output.Append(CommentChar);

                    if (index == start)
                        output.Append(NewLine);
                    else
                        output.Append(' ').Append(comment.Substring(start, index - start)).Append(NewLine);

                    if (index < comment.Length && comment[index] == '\r' && comment[index + 1] == '\n')
                        index++;
                    index++;
                    start = index;

                    if (index < comment.Length)
                    {
                        index = comment.IndexOfAny(CommentSeparators, index);
                        // is this the last line, without the new-line at the end?
                        if (index == -1)
                        {
                            output.Append(CommentChar).Append(' ').Append(comment.Substring(start)).Append(NewLine);
                            break;
                        }
                    }
                    else
                    {
                        index = comment.Length;
                    }
                }
            }
            else
            {
                output.Append(CommentChar).Append(' ').Append(comment).Append(NewLine);
            }
        }

        /// <summary>
        /// Serialize multiline comment.
        /// </summary>
        internal static void WriteComment(TextWriter output, string comment)
        {
            if (string.IsNullOrEmpty(comment))
                return;

            int index = comment.IndexOfAny(CommentSeparators);
            if (index >= 0)
            {
                // serialize multiline comment, line by line:
                int start = 0;
                while (index < comment.Length)
                {
                    output.Write(CommentChar);

                    if (index == start)
                        output.WriteLine();
                    else
                    {
                        output.Write(' ');
                        output.WriteLine(comment.Substring(start, index - start));
                    }

                    if (index < comment.Length && comment[index] == '\r' && comment[index + 1] == '\n')
                        index++;
                    index++;
                    start = index;

                    if (index < comment.Length)
                    {
                        index = comment.IndexOfAny(CommentSeparators, index);
                        // is this the last line, without the new-line at the end?
                        if (index == -1)
                        {
                            output.Write(CommentChar);
                            output.Write(' ');
                            output.WriteLine(comment.Substring(start));
                            break;
                        }
                    }
                    else
                    {
                        index = comment.Length;
                    }
                }
            }
            else
            {
                output.Write(CommentChar);
                output.Write(' ');
                output.WriteLine(comment);
            }
        }
    }
}
