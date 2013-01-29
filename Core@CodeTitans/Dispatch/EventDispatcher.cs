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
    /// <summary>
    /// Factory for dispatchers for given platform or .NET version.
    /// The idea here is to create an abstraction layer above event notifications,
    /// that should finally be processed inside the GUI owned thread.
    /// </summary>
    public static class EventDispatcher
    {
        /// <summary>
        /// Creates new dispatcher class to call on the current thread context.
        /// </summary>
        public static IEventDispatcher Create()
        {
            return new DefaultDispatcher();
        }

#if !WINDOWS_PHONE && !SILVERLIGHT && !DISABLE_WINDOWS_FORMS && !WINDOWS_STORE
        /// <summary>
        /// Creates new dispatcher based on given Windows.Forms control.
        /// </summary>
        public static IEventDispatcher Create(System.Windows.Forms.Control control)
        {
            return new WindowsFormsDispatcher(control);
        }
#endif

#if !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE && !NET_2_COMPATIBLE
        /// <summary>
        /// Creates new dispatcher based on given System.Windows dependency object.
        /// </summary>
        public static IEventDispatcher Create(System.Windows.DependencyObject dependencyObject, System.Windows.Threading.DispatcherPriority priority)
        {
            return new DependencyObjectDispatcher(dependencyObject, priority);
        }

        /// <summary>
        /// Creates new dispatcher based on given System.Windows dispatcher object.
        /// </summary>
        public static IEventDispatcher Create(System.Windows.Threading.Dispatcher dispatcher, System.Windows.Threading.DispatcherPriority priority)
        {
            return new DependencyObjectDispatcher(dispatcher, priority);
        }
#endif

#if !WINDOWS_STORE && !NET_2_COMPATIBLE
        /// <summary>
        /// Creates new dispatcher based on given System.Windows dependency object.
        /// </summary>
        public static IEventDispatcher Create(System.Windows.DependencyObject dependencyObject)
        {
            return new DependencyObjectDispatcher(dependencyObject);
        }

        /// <summary>
        /// Creates new dispatcher based on given System.Windows dispatcher object.
        /// </summary>
        public static IEventDispatcher Create(System.Windows.Threading.Dispatcher dispatcher)
        {
            return new DependencyObjectDispatcher(dispatcher);
        }
#endif
    }
}
