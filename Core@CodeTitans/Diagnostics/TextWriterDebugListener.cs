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
        private readonly bool _printTime;
        private readonly char[] _time;

        /// <summary>
        /// Gets the name of this trace listener.
        /// </summary>
        public const string ListenerName = "Persistent Store Output";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public TextWriterDebugListener(TextWriter output, bool printTime)
        {
            if (output == null)
                throw new ArgumentNullException("output");

            _output = output;
            _printTime = printTime;

            if (printTime)
            {
                _time = CreateTimeArray();
            }
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public TextWriterDebugListener(TextWriter output)
            : this(output, false)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public TextWriterDebugListener(Stream stream, Encoding encoding, bool printTime)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _output = new StreamWriter(stream, encoding);
            _printTime = printTime;
            if (printTime)
            {
                _time = CreateTimeArray();
            }
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public TextWriterDebugListener(Stream stream, Encoding encoding)
            : this(stream, encoding, false)
        {
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

        /// <summary>
        /// Writes a debug entry into the log.
        /// </summary>
        public void WriteLine(DebugEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Message))
                return;
            if (_output == null)
                return;

            if (_printTime)
            {
                UpdateTimeArray(entry);
                _output.Write(_time);
            }

            _output.WriteLine(entry.Message);

            if (!string.IsNullOrEmpty(entry.StackTrace))
            {
                _output.WriteLine(entry.StackTrace);
            }
        }

        /// <summary>
        /// Flushes content to the stream.
        /// </summary>
        public void Flush()
        {
            if (_output != null)
            {
                _output.Flush();
            }
        }

        private static char[] CreateTimeArray()
        {
            return new[] { '0', '0', ':', '0', '0', ':', '0', '0', '.', '0', '0', '0', ':', ' ' };
        }

        private void UpdateTimeArray(DebugEntry entry)
        {
            DateTime time = entry.LogTime;

            _time[0] = time.Hour < 10 ? '0' : (char) ('0' + (time.Hour / 10));
            _time[1] = (char) ('0' + (time.Hour % 10));

            _time[3] = time.Minute < 10 ? '0' : (char)('0' + (time.Minute / 10));
            _time[4] = (char)('0' + (time.Minute % 10));

            _time[6] = time.Second < 10 ? '0' : (char)('0' + (time.Second / 10));
            _time[7] = (char)('0' + (time.Second % 10));

            int millisecond = time.Millisecond;
            _time[11] = (char)('0' + (millisecond % 10));
            millisecond /= 10;
            _time[10] = (char)('0' + (millisecond % 10));
            millisecond /= 10;
            _time[9] = (char)('0' + (millisecond % 10));
        }

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_output != null)
            {
                ((IDisposable)_output).Dispose();
                _output = null;
            }
        }

        #endregion
    }
}
