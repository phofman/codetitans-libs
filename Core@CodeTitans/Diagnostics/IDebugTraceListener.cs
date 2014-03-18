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

namespace CodeTitans.Diagnostics
{
    /// <summary>
    /// Interface defining trace listeners behaviour.
    /// </summary>
#if DEBUGLOG_PUBLIC
    public
#else
    internal
#endif
    interface IDebugTraceListener
    {
        /// <summary>
        /// Gets the name of this trace listener.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Writes a debug entry into the log.
        /// </summary>
        void WriteLine(DebugEntry entry);
    }
}
