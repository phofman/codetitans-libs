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
using System.IO;
using System.Text;
using CodeTitans.Helpers;

namespace CodeTitans.Core.Generics
{
    /// <summary>
    /// Class for parsing markers out of the given stream or text.
    /// Marker is a string encapsulated within 'start' and 'end' tags.
    /// In the real scenario, they can be used to create a simple templates, where markers
    /// are used to insert a dynamic content at runtime.
    /// 
    /// Example:
    ///  'Hello %name%, how are you?'
    /// 
    /// Marker here is 'name' surrounded by '% tags. Parser will give 3 notification
    /// (with text 'Hello ', marker and on text ', how are you?')
    /// </summary>
    public static class MarkerStrings
    {
        /// <summary>
        /// Definition of the callback called on each text or marker found.
        /// </summary>
        public delegate void Callback(object o, string text);

        /// <summary>
        /// Parses data from a given reader. It returns the number of markers found.
        /// </summary>
        public static int Parse(TextReader reader, object o, string startTag, string endTag, Callback onText, Callback onMarker)
        {
            return Parse(StringHelper.CreateReader(reader), o, startTag, endTag, onText, onMarker);
        }

        /// <summary>
        /// Parses data from a given text. It returns the number of markers found.
        /// </summary>
        public static int Parse(string text, object o, string startTag, string endTag, Callback onText, Callback onMarker)
        {
            return Parse(StringHelper.CreateReader(text), o, startTag, endTag, onText, onMarker);
        }

        private static int Parse(IStringReader input, object o, string startTag, string endTag, Callback onText, Callback onMarker)
        {
            if (input == null)
                throw new ArgumentNullException("input");
            if (string.IsNullOrEmpty(startTag))
                throw new ArgumentNullException("startTag");
            if (string.IsNullOrEmpty(endTag))
                throw new ArgumentNullException("endTag");
            if (onText == null && onMarker == null)
                throw new ArgumentNullException("onText");

            if (input.IsEmpty)
                return 0;

            string line;
            string substring;
            int processingStart;
            int startIndex;
            int endIndex;
            int result = 0;
            StringBuilder text = new StringBuilder();
            StringBuilder content = new StringBuilder();
            bool insideTag = false;
            bool continueProcessing;


            while ((line = input.ReadLine()) != null)
            {
                processingStart = 0;

                do
                {
                    // is there any tag in current line:
                    startIndex = insideTag ? -1 : line.IndexOf(startTag, processingStart, StringComparison.Ordinal);
                    continueProcessing = false;

                    if (startIndex < 0)
                    {
                        if (insideTag)
                        {
                            endIndex = line.IndexOf(endTag, processingStart, StringComparison.Ordinal);

                            if (endIndex < 0)
                            {
                                // add content of the tag
                                content.Append(processingStart > 0 ? line.Substring(processingStart) : line).Append("\r\n");
                            }
                            else
                            {
                                // was there any text before
                                if (text.Length > 0)
                                {
                                    if (onText != null)
                                        onText(o, text.ToString());
#if NET_2_COMPATIBLE
                                    text.Remove(0, text.Length);
#else
                                    text.Clear();
#endif
                                }

                                // append beginning as a tag
                                if (content.Length > 0)
                                {
                                    substring = content.Append(line.Substring(processingStart, endIndex - processingStart)).ToString();
#if NET_2_COMPATIBLE
                                    content.Remove(0, text.Length);
#else
                                    content.Clear();
#endif
                                }
                                else
                                {
                                    substring = line.Substring(processingStart, endIndex - processingStart);
                                }

                                if (onMarker != null)
                                    onMarker(o, substring);

                                result++;
                                insideTag = false;
                                continueProcessing = true;
                                processingStart = endIndex + endTag.Length;
                            }
                        }
                        else
                        {
                            // add the whole line into the buffer, so we minimize the number of notifications
                            text.Append(processingStart > 0 ? line.Substring(processingStart) : line);

                            if (!input.IsEof)
                                text.Append("\r\n");
                        }
                    }
                    else
                    {
                        // text before tag
                        if (startIndex > processingStart || text.Length > 0)
                        {
                            if (text.Length > 0)
                            {
                                substring = text.Append(line.Substring(processingStart, startIndex - processingStart)).ToString();
#if NET_2_COMPATIBLE
                                text.Remove(0, text.Length);
#else
                                text.Clear();
#endif
                            }
                            else
                                substring = line.Substring(processingStart, startIndex - processingStart);

                            if (onText != null)
                                onText(o, substring);
                        }

                        // tag:
                        endIndex = line.IndexOf(endTag, startIndex + startTag.Length, StringComparison.Ordinal);
                        if (endIndex < 0)
                        {
                            // add rest of the line as the content of the tag
                            content.Append(line.Substring(startIndex + startTag.Length)).Append("\r\n");
                            insideTag = true;
                        }
                        else
                        {
                            if (content.Length > 0)
                            {
                                substring = content.Append(line.Substring(startIndex + startTag.Length, endIndex - startIndex - endTag.Length)).ToString();
#if NET_2_COMPATIBLE
                                content.Remove(0, text.Length);
#else
                                content.Clear();
#endif
                            }
                            else
                            {
                                substring = line.Substring(startIndex + startTag.Length, endIndex - startIndex - endTag.Length);
                            }

                            if (onMarker != null)
                                onMarker(o, substring);

                            result++;
                            insideTag = false;
                            continueProcessing = true;
                            processingStart = endIndex + endTag.Length;
                        }
                    }
                } while (continueProcessing);
            }

            if (insideTag || content.Length > 0)
                throw new FormatException(string.Concat("Unclosed tag with content: '", content, "'"));

            if (onText != null && text.Length > 0)
                onText(o, text.ToString());

            return result;
        }
    }
}
