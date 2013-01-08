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
using System.ComponentModel;
using System.Threading;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core
#endif
{
    /// <summary>
    /// Helper class for invoking events in thread-safe manner.
    /// </summary>
#if CODETITANS_LIB_CORE
    public
#endif
    static class Event
    {
        /// <summary>
        /// Invokes event with given parameter.
        /// </summary>
        public static void Invoke<T>(EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
#if !DISABLE_INTERLOCKED
            EventHandler<T> eh = Interlocked.CompareExchange(ref eventHandler, null, null);
#else
            EventHandler<T> eh = eventHandler;
#endif
            if (eh != null)
                eh(sender, e);
        }

#if !NET_2_COMPATIBLE
        /// <summary>
        /// Invokes event with given parameter.
        /// </summary>
        public static void Invoke(System.ComponentModel.PropertyChangedEventHandler eventHandler, object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
#if !DISABLE_INTERLOCKED
            System.ComponentModel.PropertyChangedEventHandler eh = Interlocked.CompareExchange(ref eventHandler, null, null);
#else
            System.ComponentModel.PropertyChangedEventHandler eh = eventHandler;
#endif
            if (eh != null)
                eh(sender, e);
        }

        /// <summary>
        /// Invokes event with given parameter.
        /// </summary>
        public static void Invoke(System.ComponentModel.PropertyChangedEventHandler eventHandler, object sender, string propertyName)
        {
            Invoke(eventHandler, sender, new PropertyChangedEventArgs(propertyName));
        }

#if CODETITANS_LIB_CORE
        /// <summary>
        /// Invokes event with given parameter.
        /// </summary>
        public static void Invoke(System.Collections.Specialized.NotifyCollectionChangedEventHandler eventHandler, object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
#if !DISABLE_INTERLOCKED
            System.Collections.Specialized.NotifyCollectionChangedEventHandler eh = Interlocked.CompareExchange(ref eventHandler, null, null);
#else
            System.Collections.Specialized.NotifyCollectionChangedEventHandler eh = eventHandler;
#endif
            if (eh != null)
                eh(sender, e);
        }
#endif // CODETITANS_LIB_CORE

#endif // !NET_2_COMPATIBLE

        private class InvokeDelayedState<T> where T : EventArgs
        {
            public InvokeDelayedState(int delay, EventHandler<T> eventHandler, object sender, T args)
            {
                Delay = delay;
                EventHandler = eventHandler;
                Sender = sender;
                Args = args;
            }

            #region Properties

            public int Delay
            {
                get;
                private set;
            }

            public EventHandler<T> EventHandler
            {
                get;
                private set;
            }

            public object Sender
            {
                get;
                private set;
            }

            public T Args
            {
                get;
                private set;
            }

            public void Invoke()
            {
                Event.Invoke(EventHandler, Sender, Args);
            }

            #endregion
        }

        /// <summary>
        /// Invokes an event with given parameters and a given dalay.
        /// Currently it uses default ThreadPool, so the number of calls is limited to its the maximum number of threads.
        /// </summary>
        public static void InvokeDelayed<T>(int delay, EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
#if !DISABLE_INTERLOCKED
            EventHandler<T> eh = Interlocked.CompareExchange(ref eventHandler, null, null);
#else
            EventHandler<T> eh = eventHandler;
#endif
            if (eh != null)
            {
                if (delay <= 0)
                    eh(sender, e);
                else
                {
                    InternalInvokeDelayed(new InvokeDelayedState<T>(delay, eventHandler, sender, e));
                }
            }
        }

#if WINDOWS_STORE
        private static async void InternalInvokeDelayed<T>(InvokeDelayedState<T> state) where T : EventArgs
        {
            await System.Threading.Tasks.Task.Delay(state.Delay);
            state.Invoke();
        }
#else
        private static void InternalInvokeDelayed<T>(InvokeDelayedState<T> state) where T : EventArgs
        {
            ThreadPool.QueueUserWorkItem(InternalPerformDelayedInvoke<T>, state);
        }

        private static void InternalPerformDelayedInvoke<T>(object s) where T : EventArgs
        {
            var state = (InvokeDelayedState<T>)s;

            Thread.Sleep(state.Delay);
            state.Invoke();
        }
#endif
    }
}
