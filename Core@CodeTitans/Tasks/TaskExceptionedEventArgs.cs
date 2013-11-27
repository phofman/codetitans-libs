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
    /// Arguments associated with unhandled exception caught when executing task.
    /// </summary>
    public sealed class TaskExceptionedEventArgs : TaskExecutorEventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public TaskExceptionedEventArgs(ITaskExecutor executor, ITask task, Exception ex)
            : base(executor, false)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (ex == null)
                throw new ArgumentNullException("ex");

            Task = task;
            Exception = ex;
        }

        #region Properties

        /// <summary>
        /// Gets the task associated with unhandled exception caught.
        /// </summary>
        public ITask Task
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the exception associated with last executed task.
        /// </summary>
        public Exception Exception
        {
            get;
            private set;
        }

        /// <summary>
        /// Updates the status if the exception was handled or not.
        /// </summary>
        public bool Handled
        {
            get;
            set;
        }

        #endregion
    }
}
