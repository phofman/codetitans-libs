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
using CodeTitans.Helpers;

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Class representing simple INI file from Windows.
    /// </summary>
    public sealed class IniStrings : IEnumerable<IniSection>
    {
        private readonly List<string> _names;
        private readonly List<IniSection> _sections;
        private readonly IDictionary<string, IniSection> _sectionDir;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IniStrings()
        {
            _names = new List<string>();
            _sections = new List<IniSection>();
            _sectionDir = new Dictionary<string, IniSection>();
        }

        #region Properties

        /// <summary>
        /// Gets the list of sections defined in the source.
        /// </summary>
        public IniSection[] Sections
        {
            get { return _sections.ToArray(); }
        }

        /// <summary>
        /// Gets the section with specified name.
        /// Names are case insensitive.
        /// </summary>
        public IniSection this[string name]
        {
            get
            {
                IniSection result;
                return TryGetValue(name, out result) ? result : null;
            }
        }

        /// <summary>
        /// Gets the list of section names.
        /// </summary>
        public string[] Names
        {
            get { return _names.ToArray(); }
        }

        /// <summary>
        /// Gets the number of defined sections.
        /// </summary>
        public int Count
        {
            get { return _sections.Count; }
        }

        #endregion

        /// <summary>
        /// Adds new section.
        /// </summary>
        public void Add(IniSection section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            string lowerName = string.IsNullOrEmpty(section.Name) ? string.Empty : section.Name.ToLower();
            IniSection existingSection;

            if (_sectionDir.TryGetValue(lowerName, out existingSection))
            {
                existingSection.OverwriteWith(section);
            }
            else
            {
                _sectionDir.Add(lowerName, section);
                _sections.Add(section);
                _names.Add(section.Name);
            }
        }

        /// <summary>
        /// Adds a collection of sections.
        /// </summary>
        public void Add(IEnumerable<IniSection> sections)
        {
            if (sections == null)
                throw new ArgumentNullException("sections");

            foreach (var section in sections)
                Add(section);
        }

        /// <summary>
        /// Checks if section with given name is already registered.
        /// </summary>
        public bool Contains(string name)
        {
            return _sectionDir.ContainsKey(string.IsNullOrEmpty(name) ? string.Empty : name.ToLower());
        }

        /// <summary>
        /// Gets the section with given case insensitive name.
        /// </summary>
        public bool TryGetValue(string name, out IniSection section)
        {
            return _sectionDir.TryGetValue(string.IsNullOrEmpty(name) ? string.Empty : name.ToLower(), out section);
        }

        /// <summary>
        /// Removes specified section.
        /// </summary>
        public bool Remove(string name)
        {
            string lowerName = string.IsNullOrEmpty(name) ? string.Empty : name.ToLower();
            IniSection section;

            // if section with given name exists, remove it from all collections
            if (_sectionDir.TryGetValue(lowerName, out section))
            {
                _names.Remove(lowerName);
                _sections.Remove(section);
                _sectionDir.Remove(lowerName);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all stored sections.
        /// </summary>
        public void Clear()
        {
            _sectionDir.Clear();
            _sections.Clear();
            _names.Clear();
        }

        /// <summary>
        /// Writes all stored data to given output.
        /// </summary>
        public void Write(StringBuilder output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            foreach (var section in _sections)
            {
                section.Write(output);
                output.Append(IniSection.NewLine);
            }
        }

        /// <summary>
        /// Writes all stored data to given output.
        /// </summary>
        public void Write(TextWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            foreach (var section in _sections)
            {
                section.Write(output);
                output.WriteLine();
            }
        }

        /// <summary>
        /// Reads ini file definition from given text.
        /// </summary>
        public static IniStrings Read(String text)
        {
            return Read(StringHelper.CreateReader(text));
        }

        /// <summary>
        /// Reads ini file definition from given text.
        /// </summary>
        public static IniStrings Read(TextReader reader)
        {
            return Read(StringHelper.CreateReader(reader));
        }

        private static IniStrings Read(IStringReader reader)
        {
            if (reader.IsEof)
                return null;

            IniStrings result = new IniStrings();
            IniSection section = null;
            StringBuilder comment = new StringBuilder();
            int index;

            while (!reader.IsEof)
            {
                var line = reader.ReadLine();
                if (line == null)
                    break;
                line = line.Trim();
                if (line.Length == 0)
                    continue;

                /////////////////////////////////////
                // what kind of line is this:

                // a comment?
                if (line[0] == IniSection.CommentChar || line[0] == IniSection.CommentCharAlt)
                {
                    if (comment.Length > 0)
                        comment.Append(IniSection.NewLine);
                    comment.Append(line.Substring(1).Trim());
                    continue;
                }

                // a section header?
                if (line[0] == IniSection.SectionStart && line[line.Length - 1] == IniSection.SectionEnd)
                {
                    var name = line.Substring(1, line.Length - 2).Trim();
                    var summary = comment.ToString();

                    comment.Remove(0, comment.Length);
                    section = new IniSection(name, summary);
                    result.Add(section);
                    continue;
                }

                index = line.IndexOf('=');
                if (index >= 0)
                {
                    var name = line.Substring(0, index).Trim();
                    var value = index == line.Length - 1 ? string.Empty : line.Substring(index + 1).Trim();
                    var summary = comment.ToString();

                    comment.Remove(0, comment.Length);

                    if (value.Length >= 2 && ((value[0] == '"' && value[value.Length - 1] == '"') || (value[0] == '\'' && value[value.Length - 1] == '\'')))
                        value = value.Substring(1, value.Length - 2);

                    if (section == null)
                        section = new IniSection(string.Empty);

                    section.Add(new IniSectionItem(name, value, summary));

                    continue;
                }

                throw new FormatException(string.Concat("Invalid line ", reader.Line, " found: '", line, "'"));
            }

            return result;
        }

        #region IEnumerator implementation

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IniSection> GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            Write(result);
            return result.ToString();
        }
    }
}
