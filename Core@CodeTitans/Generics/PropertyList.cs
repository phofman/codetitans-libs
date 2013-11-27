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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using CodeTitans.Core.Generics.Objects;
using CodeTitans.Diagnostics;
using CodeTitans.Helpers;

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Class representing MacOS XML property list file.
    /// </summary>
    public sealed class PropertyList : IPropertyListDictionary
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public PropertyList()
        {
            Root = new PropertyListDictionary(null);
            Version = new Version(1, 0);
        }

        /// <summary>
        /// Hidden constructor. For Read() only usage.
        /// </summary>
        private PropertyList(IPropertyListItem root, Version version)
        {
            if (root == null)
                throw new ArgumentNullException("root");
            if (version == null)
                throw new ArgumentNullException("version");

            Root = root;
            Version = version;
        }

        #region Properties

        /// <summary>
        /// Gets the root element of the settings.
        /// </summary>
        public IPropertyListItem Root
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public Version Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the element at given index.
        /// </summary>
        public IPropertyListItem this[Int32 index]
        {
            get { return Root[index]; }
        }

        /// <summary>
        /// Gets the number of stored items.
        /// </summary>
        public Int32 Count
        {
            get { return Root.Count; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// </summary>
        public IPropertyListItem this[String key]
        {
            get { return Root[key]; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        public IPropertyListItem this[String key, Int32 defaultValue]
        {
            get { return Root[key, defaultValue]; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        public IPropertyListItem this[String key, String defaultValue]
        {
            get { return Root[key, defaultValue]; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        public IPropertyListItem this[String key, Double defaultValue]
        {
            get { return Root[key, defaultValue]; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        public IPropertyListItem this[String key, Byte[] defaultValue]
        {
            get { return Root[key, defaultValue]; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        public IPropertyListItem this[String key, DateTime defaultValue]
        {
            get { return Root[key, defaultValue]; }
        }

        /// <summary>
        /// Gets the member element with given name.
        /// If it doesn't exists the default value is returned.
        /// </summary>
        public IPropertyListItem this[String key, Boolean defaultValue]
        {
            get { return Root[key, defaultValue]; }
        }

        /// <summary>
        /// Gets the collection of all internally stored key names.
        /// </summary>
        public ICollection<String> Keys
        {
            get { return Root.Keys; }
        }

        /// <summary>
        /// Gets the enumerable collection of elements.
        /// </summary>
        public IEnumerable<KeyValuePair<string, IPropertyListItem>> DictionaryItems
        {
            get { return Root.DictionaryItems; }
        }

        #endregion

        #region IPropertyListDictionary

        /// <summary>
        /// Checks if member element with given name exists.
        /// </summary>
        public bool Contains(String key)
        {
            return Root.Contains(key);
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        public IPropertyListItem Add(String key, Int32 value)
        {
            return Root.Add(key, value);
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        public IPropertyListItem Add(String key, String value)
        {
            return Root.Add(key, value);
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        public IPropertyListItem Add(String key, Double value)
        {
            return Root.Add(key, value);
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        public IPropertyListItem Add(String key, Byte[] value)
        {
            return Root.Add(key, value);
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        public IPropertyListItem Add(String key, DateTime value)
        {
            return Root.Add(key, value);
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        public IPropertyListItem Add(String key, Boolean value)
        {
            return Root.Add(key, value);
        }

        /// <summary>
        /// Adds new dictionary item.
        /// </summary>
        public IPropertyListItem AddNewDictionary(String key)
        {
            return Root.AddNewDictionary(key);
        }

        /// <summary>
        /// Adds new array.
        /// </summary>
        public IPropertyListItem AddNewArray(String key)
        {
            return Root.AddNewArray(key);
        }

        /// <summary>
        /// Removes item at given key.
        /// Returns that item or null if didn't exist.
        /// </summary>
        public IPropertyListItem Remove(String key)
        {
            return Root.Remove(key);
        }

        /// <summary>
        /// Removes all items.
        /// </summary>
        public void Clear()
        {
            Root.Clear();
        }

        /// <summary>
        /// Removes an item at given index.
        /// Returns removed object or null in case index was invalid.
        /// </summary>
        public IPropertyListItem RemoveAt(Int32 index)
        {
            return Root.RemoveAt(index);
        }

        #endregion

        /// <summary>
        /// Writes current property list definition into a designated output.
        /// </summary>
        public void Write(StringBuilder output, Boolean indented)
        {
            Write(output, indented, Root);
        }

        /// <summary>
        /// Writes current property list definition into a designated output.
        /// </summary>
        public void Write(StringBuilder output)
        {
            Write(output, true, Root);
        }

        /// <summary>
        /// Writes current property list definition into a designated output.
        /// </summary>
        public void Write(TextWriter output, Boolean indented)
        {
            Write(output, indented, Root);
        }

        /// <summary>
        /// Writes current property list definition into a designated output.
        /// </summary>
        public void Write(TextWriter output)
        {
            Write(output, true, Root);
        }

        /// <summary>
        /// Writes current property list definition into a designated output.
        /// </summary>
        public void Write(XmlWriter output)
        {
            Write(output, Root);
        }

        /// <summary>
        /// Gets the string representation of this object.
        /// </summary>
        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            Write(result, true);
            return result.ToString();
        }

        /// <summary>
        /// Reads the texts and interprets as PList definition.
        /// </summary>
        public static PropertyList Read(TextReader text)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();

#if NET_2_COMPATIBLE && !PocketPC && !WINDOWS_PHONE
                // obsolete, but will cause an exception at start-up if not set to false (crazy!)
                settings.ProhibitDtd = false;
#endif

#if !NET_2_COMPATIBLE
                settings.DtdProcessing = DtdProcessing.Ignore;
#endif
                settings.IgnoreComments = true;
                settings.IgnoreProcessingInstructions = true;
                settings.IgnoreWhitespace = true;

                Version resultVersion = null;
                IPropertyListItem result = null;
                int intNumber;
                double doubleNumber;
                string key = null;
                Stack<IPropertyListItem> items = new Stack<IPropertyListItem>();

                using (XmlReader reader = XmlReader.Create(text, settings))
                {
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (reader.Name == "plist")
                                {
                                    if (resultVersion != null)
                                        throw new FormatException("Secondary plist definition not expected");

                                    if (reader.MoveToAttribute("version"))
                                    {
                                        if (reader.ReadAttributeValue())
                                        {
                                            resultVersion = new Version(reader.Value);
                                        }
                                    }
                                    break;
                                }

                                if (reader.Name == "key")
                                {
                                    if (key != null)
                                        throw new FormatException("Two keys in a row are not expected");

                                    if (reader.Read())
                                        key = reader.Value;
                                    break;
                                }

                                if (reader.Name == "dict")
                                {
                                    if (key == null)
                                    {
                                        if (result == null)
                                        {
                                            result = new PropertyListDictionary(null);
                                            if (!reader.IsEmptyElement)
                                                items.Push(result);
                                            break;
                                        }
                                        if (items.Peek().Type != PropertyListItemTypes.Array)
                                            throw new FormatException("Found dictionary definition without key name");
                                    }

                                    // add new dictionary to the top collection:
                                    var newDictionary = items.Peek().AddNewDictionary(key);
                                    if (!reader.IsEmptyElement)
                                        items.Push(newDictionary);
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "array")
                                {
                                    if (key == null)
                                    {
                                        if (result == null)
                                        {
                                            result = new PropertyListArray(null);
                                            if (!reader.IsEmptyElement)
                                                items.Push(result);
                                            break;
                                        }
                                        if (items.Peek().Type != PropertyListItemTypes.Array)
                                            throw new FormatException("Found array definition without key name");
                                    }

                                    // add new array to the top collection:
                                    var newArray = items.Peek().AddNewArray(key);
                                    if (!reader.IsEmptyElement)
                                        items.Push(newArray);
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "string")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found string definition without key name");

                                    if (reader.Read())
                                        items.Peek().Add(key, reader.Value);
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "integer")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found integer definition without key name");

                                    if (reader.Read())
                                    {
                                        if (NumericHelper.TryParseInt32(reader.Value, out intNumber))
                                            items.Peek().Add(key, intNumber);
                                        else
                                            throw new FormatException("Can not parse number: '" + reader.Value + "'");
                                    }
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "real")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found real definition without key name");

                                    if (reader.Read())
                                    {
                                        if (NumericHelper.TryParseDouble(reader.Value, NumberStyles.Float, out doubleNumber))
                                            items.Peek().Add(key, doubleNumber);
                                        else
                                            throw new FormatException("Can not parse number: '" + reader.Value + "'");
                                    }
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "true")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found boolean definition without key name");

                                    items.Peek().Add(key, true);
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "false")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found boolean definition without key name");

                                    items.Peek().Add(key, false);
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "date")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found date definition without key name");

                                    if (reader.Read())
                                        items.Peek().Add(key, DateTime.Parse(reader.Value, CultureInfo.InvariantCulture));
                                    key = null;
                                    break;
                                }

                                if (reader.Name == "data")
                                {
                                    if (key == null && items.Peek().Type == PropertyListItemTypes.Dictionary)
                                        throw new FormatException("Found data definition without key name");

                                    if (reader.Read())
                                        items.Peek().Add(key, Convert.FromBase64String(reader.Value));
                                    key = null;
                                    break;
                                }

                                break;

                            case XmlNodeType.EndElement:
                                if (EndElement(items, reader.Name, "dict", PropertyListItemTypes.Dictionary))
                                    break;
                                if (EndElement(items, reader.Name, "array", PropertyListItemTypes.Array))
                                    break;
                                break;
                        }
                    }
                }

                if (items.Count > 0)
                    throw new FormatException("Not all items have been closed");

                return new PropertyList(result, resultVersion);
            }
            catch (Exception ex)
            {
                DebugLog.WriteCoreException(ex);
                throw;
            }
        }

        private static bool EndElement(Stack<IPropertyListItem> items, string elementName, string expectedName, PropertyListItemTypes type)
        {
            if (elementName == expectedName)
            {
                if (items != null && items.Count > 0 && items.Peek().Type == type)
                {
                    items.Pop();
                    return true;
                }

                throw new FormatException("Expected '" + expectedName + "' element closing tag");
            }

            return false;
        }

        /// <summary>
        /// Reads the texts and interprets as PList definition.
        /// </summary>
        public static PropertyList Read(String text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            using (TextReader input = new StringReader(text))
            {
                return Read(input);
            }
        }

        /// <summary>
        /// Creates the default settings used during writing.
        /// </summary>
        private static XmlWriterSettings CreateWriteSettings(Boolean indented)
        {
            XmlWriterSettings settings = new XmlWriterSettings();

            settings.Encoding = Encoding.UTF8;
            settings.Indent = indented;
            settings.IndentChars = "    ";
#if PocketPC
            settings.NewLineChars = "\r\n";
#else
            settings.NewLineChars = Environment.NewLine;
#endif

            return settings;
        }

        /// <summary>
        /// Writes given property list into a designated output.
        /// </summary>
        public static void Write(TextWriter output, Boolean indented, IPropertyListItem root)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            using (XmlWriter writer = XmlWriter.Create(output, CreateWriteSettings(indented)))
            {
                Write(writer, root);
            }
        }

        /// <summary>
        /// Writes given property list into a designated output.
        /// </summary>
        public static void Write(StringBuilder output, Boolean indented, IPropertyListItem root)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            using (XmlWriter writer = XmlWriter.Create(output, CreateWriteSettings(indented)))
            {
                Write(writer, root);
            }
        }

        /// <summary>
        /// Writes given property list into a designated output.
        /// </summary>
        public static void Write(XmlWriter writer, IPropertyListItem root)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteStartDocument();
            writer.WriteDocType("plist", "-//Apple Computer//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
            writer.WriteStartElement("plist");
            writer.WriteAttributeString("version", "1.0");
            WriteItem(writer, root);
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private static void WriteItem(XmlWriter writer, IPropertyListItem item)
        {
            switch (item.Type)
            {
                case PropertyListItemTypes.Dictionary:
                    writer.WriteStartElement("dict");
                    foreach (KeyValuePair<string, IPropertyListItem> childItem in item.DictionaryItems)
                    {
                        writer.WriteStartElement("key");
                        writer.WriteString(childItem.Key);
                        writer.WriteEndElement();
                        WriteItem(writer, childItem.Value);
                    }
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.Array:
                    writer.WriteStartElement("array");
                    foreach (IPropertyListItem childItem in item.ArrayItems)
                        WriteItem(writer, childItem);
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.Binary:
                    writer.WriteStartElement("data");
                    writer.WriteBase64(item.BinaryValue, 0, item.BinaryValue.Length);
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.Boolean:
                    if (item.BooleanValue)
                        writer.WriteStartElement("true");
                    else
                        writer.WriteStartElement("false");
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.DateTime:
                    writer.WriteStartElement("date");
                    writer.WriteString(item.DateTimeValue.ToString("s", CultureInfo.InvariantCulture));
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.String:
                    writer.WriteStartElement("string");
                    writer.WriteString(item.StringValue);
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.FloatingNumber:
                    writer.WriteStartElement("real");
                    writer.WriteString(item.StringValue);
                    writer.WriteEndElement();
                    break;

                case PropertyListItemTypes.IntegerNumber:
                    writer.WriteStartElement("integer");
                    writer.WriteString(item.StringValue);
                    writer.WriteEndElement();
                    break;

                default:
                    throw new InvalidOperationException("Serialization action not defined for given item type (" + item.Type + ")!");
            }
        }
    }
}
