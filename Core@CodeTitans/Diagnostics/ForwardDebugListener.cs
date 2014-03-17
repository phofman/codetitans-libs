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
    /// <summary>
    /// Class, that forwards particular debug log messages, matching filtering criterias, to specified underlying listeners.
    /// </summary>
#if DEBUGLOG_PUBLIC
    public
#else
    internal
#endif
    sealed class ForwardDebugListener : IDebugTraceListener
    {
        private readonly List<IDebugTraceListener> _listeners;
        private readonly List<string> _includes;
        private readonly List<string> _excludes;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ForwardDebugListener(string name)
        {
            Name = name;
            _listeners = new List<IDebugTraceListener>();
            _includes = new List<string>();
            _excludes = new List<string>();
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ForwardDebugListener(string name, IDebugTraceListener listener)
        {
            Name = name;
            _listeners = new List<IDebugTraceListener>();
            _includes = new List<string>();
            _excludes = new List<string>();

            AddListener(listener);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ForwardDebugListener(string name, IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            Name = name;
            _listeners = new List<IDebugTraceListener>();
            _includes = new List<string>();
            _excludes = new List<string>();

            AddInclude(includes);
            AddExclude(excludes);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ForwardDebugListener(string name, IDebugTraceListener listener, IEnumerable<string> includes)
        {
            Name = name;
            _listeners = new List<IDebugTraceListener>();
            _includes = new List<string>();
            _excludes = new List<string>();

            AddInclude(includes);
            AddListener(listener);
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ForwardDebugListener(string name, IDebugTraceListener listener, IEnumerable<string> includes, IEnumerable<string> excludes)
        {
            Name = name;
            _listeners = new List<IDebugTraceListener>();
            _includes = new List<string>();
            _excludes = new List<string>();

            AddInclude(includes);
            AddExclude(excludes);
            AddListener(listener);
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
            if (MatchesCriteria(entry.Category))
            {
                foreach (var listener in _listeners)
                {
                    listener.WriteLine(entry);
                }
            }
        }

        private bool MatchesCriteria(string category)
        {
            if (string.IsNullOrEmpty(category))
                return false;

            // is it on 'excluded' list?
            if (_excludes.Count > 0)
            {
                foreach (var exclude in _excludes)
                {
                    if (category.StartsWith(exclude, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            // is it on 'included' list?
            if (_includes.Count > 0)
            {
                foreach (var include in _includes)
                {
                    if (category.StartsWith(include, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Gets the enumeration of currently registered listeners.
        /// </summary>
        public IEnumerable<IDebugTraceListener> Listeners
        {
            get { return _listeners; }
        }

        /// <summary>
        /// Gets the enumeration of current 'include' categories.
        /// </summary>
        public IEnumerable<string> Includes
        {
            get { return _includes; }
        }

        /// <summary>
        /// Gets the enumeration of current 'exclude' categories.
        /// </summary>
        public IEnumerable<string> Excludes
        {
            get { return _excludes; }
        }

        /// <summary>
        /// Gets the number of stored listeners the logs are forwarded to.
        /// </summary>
        public int Count
        {
            get { return _listeners.Count; }
        }

        /// <summary>
        /// Adds a listener to forward it debug log messages.
        /// </summary>
        public void AddListener(IDebugTraceListener listener)
        {
            if (listener != null)
            {
                // to be sure, nothing is debug-printed at the moment
                lock (typeof(DebugLog))
                {
                    _listeners.Add(listener);
                }
            }
        }

        /// <summary>
        /// Removes stored listener.
        /// </summary>
        public bool RemoveListener(IDebugTraceListener listener)
        {
            if (listener != null)
            {
                lock (typeof(DebugLog))
                {
                    return _listeners.Remove(listener);
                }
            }

            return false;
        }

        /// <summary>
        /// Removes all stored listeners.
        /// </summary>
        public void RemoveListeners()
        {
            lock (typeof(DebugLog))
            {
                _listeners.Clear();
            }
        }

        /// <summary>
        /// Updates the filtering. All debug log entries starting with specified category will be forwarded.
        /// </summary>
        public void AddInclude(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                lock (typeof(DebugLog))
                {
                    _includes.Add(category);
                }
            }
        }

        /// <summary>
        /// Updates the filtering. All debug log entries starting with specified categories will be forwarded.
        /// </summary>
        public void AddInclude(IEnumerable<string> categories)
        {
            if (categories != null)
            {
                lock (typeof(DebugLog))
                {
                    _includes.AddRange(categories);
                }
            }
        }

        /// <summary>
        /// Removes stored category.
        /// </summary>
        public void RemoveInclude(string category)
        {
            if (category != null)
            {
                lock (typeof(DebugLog))
                {
                    _includes.Remove(category);
                }
            }
        }

        /// <summary>
        /// Clears whole inclusion list. Nothing will be then forwarded until new include-category items are added.
        /// </summary>
        public void RemoveIncludes()
        {
            lock (typeof(DebugLog))
            {
                _includes.Clear();
            }
        }

        /// <summary>
        /// Updates the filtering. All debug log entries starting with specified category will always be rejected.
        /// </summary>
        public void AddExclude(string category)
        {
            if (!string.IsNullOrEmpty(category))
            {
                lock (typeof(DebugLog))
                {
                    _excludes.Add(category);
                }
            }
        }

        /// <summary>
        /// Updates the filtering. All debug log entries starting with specified categories will be rejected.
        /// </summary>
        public void AddExclude(IEnumerable<string> categories)
        {
            if (categories != null)
            {
                lock (typeof(DebugLog))
                {
                    _excludes.AddRange(categories);
                }
            }
        }

        /// <summary>
        /// Removes stored category.
        /// </summary>
        public void RemoveExclude(string category)
        {
            if (category != null)
            {
                lock (typeof(DebugLog))
                {
                    _excludes.Remove(category);
                }
            }
        }

        /// <summary>
        /// Clears whole exclusion list.
        /// </summary>
        public void RemoveExcludes()
        {
            lock (typeof(DebugLog))
            {
                _excludes.Clear();
            }
        }

    }

}
