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
using System.Diagnostics;

namespace CodeTitans.Diagnostics
{
    /// <summary>
    /// Class that captures application debug logs and transfers them into standard debug output.
    /// </summary>
    internal sealed class StandardDebugListener : IDebugTraceListener
    {
        /// <summary>
        /// New line definition.
        /// </summary>
        internal const string NewLine = "\r\n";

        /// <summary>
        /// Gets the name of this trace listener.
        /// </summary>
        public const string ListenerName = "Standard Debug Output";

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
            string message = entry.Message;

            if (entry.HasStackTrace)
                message = string.Concat(message, NewLine, entry.StackTrace);

            Debug.WriteLine(message);
#if !WINDOWS_STORE
            Console.WriteLine(message);
#endif
        }
    }
}
