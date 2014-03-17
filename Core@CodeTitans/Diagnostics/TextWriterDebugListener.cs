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

namespace CodeTitans.Diagnostics
{
    /// <summary>
    /// Class that captures application debug logs and redirects them to a given stream.
    /// </summary>
#if DEBUGLOG_PUBLIC
    public
#else
    internal
#endif
    sealed class TextWriterDebugListener : IDebugTraceListener, IDisposable
    {
        private TextWriter _output;

        /// <summary>
        /// Gets the name of this trace listener.
        /// </summary>
        public const string ListenerName = "Persistent Store Output";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public TextWriterDebugListener(TextWriter output)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            _output = output;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public TextWriterDebugListener(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _output = new StreamWriter(stream, encoding);
        }

        ~TextWriterDebugListener()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the name of this trace listener.
        /// </summary>
        public string Name
        {
            get { return ListenerName; }
        }

        public void WriteLine(DebugEntry entry)
        {
            string message = entry.Message;

            if (string.IsNullOrEmpty(message))
                return;
            if (_output == null)
                return;

            if (!string.IsNullOrEmpty(entry.StackTrace))
                message = string.Concat(message, StandardDebugListener.NewLine, entry.StackTrace);

            _output.WriteLine(message);
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_output != null)
                {
                    ((IDisposable)_output).Dispose();
                    _output = null;
                }
            }
        }

        #endregion
    }
}
