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
using System.Threading;

#if !CODETITANS_LIB_CORE
using CodeTitans.Bayeux;
#else
using CodeTitans.Core;
#endif

namespace CodeTitans.Diagnostics
{
#if !WINDOWS_STORE
    /// <summary>
    /// Internal module wrapping delayed notification functionality.
    /// </summary>
#if DEBUGLOG_PUBLIC
    public
#else
    internal
#endif
    sealed class DelayingModule : IDisposable
    {
        private Timer _timer;
        private readonly IDebugTraceListener _sourceListener;
        private readonly List<DebugEntry> _entries;

        private EventHandler<DebugListenerEventArgs> _entriesAddedDelegate;

        /// <summary>
        /// Event fired, when new log entries has been added.
        /// </summary>
        public event EventHandler<DebugListenerEventArgs> EntriesAdded
        {
            add
            {
                _entriesAddedDelegate += value;

                StartTimerIfRequired();
            }
            remove
            {
                _entriesAddedDelegate -= value;

                // and stop the timer, not to try to notify no clients:
                if (_entriesAddedDelegate == null)
                    StopTimer();
            }
        }

        /// <summary>
        /// Gets the predefined value of the delay.
        /// </summary>
        public const int DefaultDelay = 1000;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DelayingModule(int delay, IDebugTraceListener sourceListener)
        {
            if (sourceListener == null)
                throw new ArgumentNullException("sourceListener");

            Delay = delay == Timeout.Infinite ? 0 : delay;
            if (Delay > 0)
                _timer = new Timer(TimerCompletedCallback, null, Timeout.Infinite, Timeout.Infinite);
            _sourceListener = sourceListener;
            _entries = new List<DebugEntry>();
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DelayingModule(IDebugTraceListener sourceListener)
            : this(DefaultDelay, sourceListener)
        {
        }

        private void TimerCompletedCallback(object state)
        {
            IsRunning = false;

            if (_entriesAddedDelegate != null)
                NotifyStoredEntriesAndClear();
        }

        private void NotifyStoredEntriesAndClear()
        {
            DebugEntry[] entries;

            lock (_timer)
            {
                entries = _entries.ToArray();
                _entries.Clear();
            }

            NotifyEntriesAdded(entries);
        }

        private void NotifyEntriesAdded(DebugEntry[] entries)
        {
            Event.Invoke(_entriesAddedDelegate, _sourceListener, new DebugListenerEventArgs(_sourceListener, entries));
        }

        /// <summary>
        /// Queues new item for later notification.
        /// </summary>
        public void Add(DebugEntry entry)
        {
            if (_timer == null)
            {
                if (_entriesAddedDelegate != null)
                    NotifyEntriesAdded(new[] { entry });
            }
            else
            {
                lock (_timer)
                {
                    _entries.Add(entry);
                }

                StartTimerIfRequired();
            }
        }

        private void StartTimerIfRequired()
        {
            if (!IsRunning && _entriesAddedDelegate != null && _entries.Count > 0)
            {
                IsRunning = true;
                _timer.Change(Delay, Timeout.Infinite);
            }
        }

        private void StopTimer()
        {
            if (IsRunning)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                IsRunning = false;
            }
        }

        /// <summary>
        /// Removes all items, that are currently queued and scheduled for notification.
        /// </summary>
        public void Clear()
        {
            if (_timer == null)
            {
                _entries.Clear();
            }
            else
            {
                // stop timer:
                StopTimer();

                // and remove stored items:
                lock (_timer)
                {
                    _entries.Clear();
                }
            }
        }

        #region Properties

        /// <summary>
        /// Gets an indication, how long it waits till notification is invoked.
        /// </summary>
        public int Delay
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the indication, if there is currently anything scheduled for further notification invocation.
        /// </summary>
        public bool IsRunning
        {
            get;
            private set;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        #endregion
    }
#endif
}
