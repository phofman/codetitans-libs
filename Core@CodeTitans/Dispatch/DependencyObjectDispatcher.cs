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

namespace CodeTitans.Core.Dispatch
{
#if !WINDOWS_STORE && !NET_2_COMPATIBLE
    /// <summary>
    /// Dispatcher class that calls methods on Windows.Forms GUI owned thread.
    /// </summary>
    internal sealed class DependencyObjectDispatcher : IEventDispatcher
    {
        private readonly System.Windows.Threading.Dispatcher _dispatcher;

#if !WINDOWS_PHONE && !SILVERLIGHT
        private readonly System.Windows.Threading.DispatcherPriority _priority;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DependencyObjectDispatcher(System.Windows.Threading.Dispatcher dispatcher, System.Windows.Threading.DispatcherPriority priority)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            _dispatcher = dispatcher;
            _priority = priority;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DependencyObjectDispatcher(System.Windows.DependencyObject dependencyObject, System.Windows.Threading.DispatcherPriority priority)
        {
            if (dependencyObject == null || dependencyObject.Dispatcher == null)
                throw new ArgumentNullException("dependencyObject");

            _dispatcher = dependencyObject.Dispatcher;
            _priority = priority;
        }
#endif

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DependencyObjectDispatcher(System.Windows.Threading.Dispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");

            _dispatcher = dispatcher;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DependencyObjectDispatcher(System.Windows.DependencyObject dependencyObject)
        {
            if (dependencyObject == null || dependencyObject.Dispatcher == null)
                throw new ArgumentNullException("dependencyObject");

            _dispatcher = dependencyObject.Dispatcher;
        }

        /// <summary>
        /// Invokes given action on a thread owned by underlying dispatcher.
        /// </summary>
        public void Invoke(ActionCallback action)
        {
            if (action != null)
            {
#if WINDOWS_PHONE || SILVERLIGHT
            if (_dispatcher.CheckAccess())
                action();
            else
                _dispatcher.BeginInvoke(action);
#else
                if (_dispatcher.HasShutdownStarted)
                    return;

                if (_dispatcher.CheckAccess())
                    action();
                else
                    _dispatcher.Invoke(action, _priority);
#endif
            }
        }

        /// <summary>
        /// Invoke given action on a thread owned by underlying dispatcher, passing single argument.
        /// </summary>
        public void Invoke<T>(T e, Action<T> action)
        {
            if (action != null)
            {
#if WINDOWS_PHONE || SILVERLIGHT
            if (_dispatcher.CheckAccess())
                action(e);
            else
                _dispatcher.BeginInvoke(action, e);
#else
                if (_dispatcher.HasShutdownStarted)
                    return;

                if (_dispatcher.CheckAccess())
                    action(e);
                else
                    _dispatcher.Invoke(action, _priority, e);
#endif
            }
        }

        /// <summary>
        /// Invoke an event on a thread owned by underlying dispatcher.
        /// </summary>
        public void Invoke<T>(EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
        {
#if WINDOWS_PHONE || SILVERLIGHT
            if (_dispatcher.CheckAccess())
                Event.Invoke(eventHandler, sender, e);
            else
                _dispatcher.BeginInvoke(new Action<EventHandler<T>, object, T>(Event.Invoke), eventHandler, sender, e);
#else
            if (_dispatcher.HasShutdownStarted)
                return;

            if (_dispatcher.CheckAccess())
                Event.Invoke(eventHandler, sender, e);
            else
                _dispatcher.Invoke(new Action<EventHandler<T>, object, T>(Event.Invoke), _priority, eventHandler, sender, e);
#endif
        }
    }
#endif
}
