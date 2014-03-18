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
    /// Arguments passed along with event, that specified event listener has received some debug log entries.
    /// </summary>
#if DEBUGLOG_PUBLIC
    public
#else
    internal
#endif
    sealed class DebugListenerEventArgs : EventArgs
    {
        public DebugListenerEventArgs(IDebugTraceListener listener, DebugEntry[] entries)
        {
            if (listener == null)
                throw new ArgumentNullException("listener");
            if (entries == null)
                throw new ArgumentNullException("entries");

            Listener = listener;
            Entries = entries;
        }

        #region Properties

        /// <summary>
        /// Gets the source debug log listener.
        /// </summary>
        public IDebugTraceListener Listener
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the recently added entries.
        /// </summary>
        public DebugEntry[] Entries
        {
            get;
            private set;
        }

        #endregion
    }
}
