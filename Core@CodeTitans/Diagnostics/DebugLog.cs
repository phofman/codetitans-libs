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
using System.Diagnostics;

namespace CodeTitans.Diagnostics
{
    /// <summary>
    /// Common class for writing logs inside whole CodeTitans libraries.
    /// </summary>
    internal static class DebugLog
    {
        /// <summary>
        /// Gets the condition define name, when debug logs are captured.
        /// </summary>
        public const string Condition = "TRACE";

        /// <summary>
        /// Name of the 'Application' category.
        /// </summary>
        public const string CategoryApp = "App";

        /// <summary>
        /// Name of the 'General' category.
        /// </summary>
        public const string CategoryGeneral = "General";

        /// <summary>
        /// Name of the 'Core' category.
        /// </summary>
        public const string CategoryCore = "Core";

        /// <summary>
        /// Name of the 'Bayeux' category.
        /// </summary>
        public const string CategoryBayeux = "Bayeux";

        /// <summary>
        /// Gets the list of available debug trace listeners.
        /// </summary>
        public static IList<IDebugTraceListener> Listeners
        {
            get;
            private set;
        }

        static DebugLog()
        {
            Listeners = new List<IDebugTraceListener>();

            // add default listener, that prints on console:
            Listeners.Add(new StandardDebugListener());
        }

        /// <summary>
        /// Writes debug log message.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteLine(string category, string message)
        {
            WriteLine(category, message, null);
        }

        /// <summary>
        /// Writes debug log message.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteLine(string category, string message, Exception exception)
        {
            var entry = new DebugEntry(category, message, exception);

            lock (typeof(DebugLog))
            {
                foreach (var traceListender in Listeners)
                    traceListender.WriteLine(entry);
            }
        }

        /// <summary>
        /// Writes exception info to the debug log.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteException(string category, Exception ex)
        {
            if (ex != null)
                WriteLine(category, string.Concat("### (", ex.GetType().Name, ") ", ex.Message), ex);
        }

        /// <summary>
        /// Writes general debug log message.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void Log(string message)
        {
            WriteLine(CategoryGeneral, message, null);
        }

        /// <summary>
        /// Writes general exception message to the log.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void Log(Exception ex)
        {
            WriteException(CategoryGeneral, ex);
        }

        /// <summary>
        /// Writes Bayeux debug message.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteBayeuxLine(string message)
        {
            WriteLine(CategoryBayeux, message, null);
        }

        /// <summary>
        /// Writes info about exception in Bayeux module.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteBayeuxException(Exception ex)
        {
            WriteException(CategoryBayeux, ex);
        }

        /// <summary>
        /// Writes Core debug message.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteCoreLine(string message)
        {
            WriteLine(CategoryCore, message, null);
        }

        /// <summary>
        /// Writes info about exception in Core module.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteCoreException(Exception ex)
        {
            WriteException(CategoryCore, ex);
        }

        /// <summary>
        /// Writes info about exception in Application module.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteAppException(Exception ex)
        {
            WriteException(CategoryApp, ex);
        }

        /// <summary>
        /// Writes Application level debug message.
        /// </summary>
        [Conditional(Condition)]
        [DebuggerStepThrough]
        public static void WriteAppLine(string message)
        {
            WriteLine(CategoryApp, message, null);
        }

        /// <summary>
        /// Breaks execution into debugger if attached and compiled for DEBUG mode.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public static void Break()
        {
            if (Debugger.IsAttached)
                Debugger.Break();
        }

        /// <summary>
        /// Adds given listener to the collections of notified ones, when new log entries are captured.
        /// </summary>
        public static void AddListener(IDebugTraceListener listener)
        {
            lock (typeof(DebugLog))
            {
                Listeners.Add(listener);
            }
        }

        /// <summary>
        /// Releases a listener with given name.
        /// </summary>
        public static bool RemoveListener(string name)
        {
            lock (typeof(DebugLog))
            {
                int index = 0;
                foreach (var listener in Listeners)
                {
                    if (string.CompareOrdinal(listener.Name, name) == 0)
                    {
                        Listeners.RemoveAt(index);
                        return true;
                    }
                    index++;
                }
            }
            return false;
        }

        /// <summary>
        /// Releases all resources used by registered debug log listeners.
        /// </summary>
        public static void RemoveListeners()
        {
            IDebugTraceListener[] copyOfListeners;

            lock (typeof(DebugLog))
            {
                copyOfListeners = new IDebugTraceListener[Listeners.Count];
                Listeners.CopyTo(copyOfListeners, 0);
                Listeners.Clear();
            }

            foreach (var listener in copyOfListeners)
            {
                var disposable = listener as IDisposable;
                if (disposable != null)
                    disposable.Dispose();
            }
        }

        /// <summary>
        /// Releases all used resources.
        /// </summary>
        public static void Dispose()
        {
            RemoveListeners();
        }
    }
}
