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

namespace CodeTitans.Core.Tasks
{
    /// <summary>
    /// Arguments passed together with ITaskExecutor events.
    /// </summary>
    public class TaskExecutorEventArgs : EventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public TaskExecutorEventArgs(ITaskExecutor executor, bool closing)
        {
            if (executor == null)
                throw new ArgumentNullException("executor");

            Executor = executor;
            Closing = closing;
        }

        #region Properties

        /// <summary>
        /// Gets the instance of the executor that is the source of notification.
        /// </summary>
        public ITaskExecutor Executor
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the indication if the task executor is about to close/stop itself.
        /// </summary>
        public bool Closing
        {
            get;
            private set;
        }

        #endregion
    }
}
