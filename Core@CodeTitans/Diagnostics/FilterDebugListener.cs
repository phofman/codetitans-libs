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

namespace CodeTitans.Diagnostics
{
#if !WINDOWS_STORE
    /// <summary>
    /// Internal debug listener class that only monitors for debug entry logs of certain category.
    /// </summary>
    internal sealed class FilterDebugListener : IDebugTraceListener, IDisposable
    {
        private readonly IEnumerable<string> _filters;
        private readonly DelayingModule _delayingModule;
        private readonly List<DebugEntry> _entries;

        /// <summary>
        /// Event fired, when new log entries has been added.
        /// </summary>
        public event EventHandler<DebugListenerEventArgs> EntriesAdded
        {
            add { _delayingModule.EntriesAdded += value; }
            remove { _delayingModule.EntriesAdded -= value; }
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public FilterDebugListener(string name, IEnumerable<string> filters)
            : this(name, filters, DelayingModule.DefaultDelay)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public FilterDebugListener(string name, string filter)
            : this(name, filter, DelayingModule.DefaultDelay)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public FilterDebugListener(string name, string filter, int delay)
            : this(name, new[] { filter }, delay)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public FilterDebugListener(string name, IEnumerable<string> filters, int delay)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
            _filters = filters;
            _delayingModule = new DelayingModule(delay, this);
            _entries = new List<DebugEntry>();
        }

        #region Implementation of IDebugTraceListener

        /// <summary>
        /// Gets the name of this trace listener.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Writes a debug entry into the log.
        /// </summary>
        public void WriteLine(DebugEntry entry)
        {
            if (MatchesFilter(entry.Category))
            {
                _entries.Add(entry);
                _delayingModule.Add(entry);
            }
        }

        private bool MatchesFilter(string category)
        {
            if (_filters == null)
                return true;
            if (string.IsNullOrEmpty(category))
                return false;

            foreach (var filter in _filters)
            {
                if (category.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Gets the stored debug log entries.
        /// </summary>
        public IEnumerable<DebugEntry> Entries
        {
            get { return _entries; }
        }

        /// <summary>
        /// Gets the number of stored debug log entries.
        /// </summary>
        public int Count
        {
            get { return _entries.Count; }
        }

        /// <summary>
        /// Removes all stored debug log entries.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
            _delayingModule.Clear();
        }

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _delayingModule.Dispose();
            Clear();
        }

        #endregion
    }
#endif
}
