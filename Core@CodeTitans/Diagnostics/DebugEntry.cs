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

namespace CodeTitans.Diagnostics
{
    /// <summary>
    /// Class wrapping single debug log entry.
    /// </summary>
#if !PocketPC
    [System.Diagnostics.DebuggerDisplay("Category={Category}, Message={Message}")]
#endif
#if DEBUGLOG_PUBLIC
    public
#else
    internal
#endif
    sealed class DebugEntry
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public DebugEntry(string category, string message, Exception exception)
        {
            LogTime = DateTime.Now;
            Category = category;
            Message = message;
            Exception = exception;
        }

        #region Properties

        /// <summary>
        /// Gets the exact time, the log entry was captured.
        /// </summary>
        public DateTime LogTime
        {
            get;
            private set;
        }

        /// <summary>
        /// Name of the category this entry belongs.
        /// </summary>
        public string Category
        {
            get;
            private set;
        }

        /// <summary>
        /// The debug log entry message.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Exception associated with the debug log entry.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the stack-trace associated with the debug log entry.
        /// </summary>
        public string StackTrace
        {
            get { return Exception != null ? Exception.StackTrace : null; }
        }

        /// <summary>
        /// Gets an indication, if stack-trace is available for this debug log entry.
        /// </summary>
        public bool HasStackTrace
        {
            get { return Exception != null && !string.IsNullOrEmpty(Exception.StackTrace); }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(Category, "::", Message);
        }
    }
}
