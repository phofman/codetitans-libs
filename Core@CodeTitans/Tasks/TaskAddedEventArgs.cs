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
    /// Arguments associated with task queued in execution queue.
    /// </summary>
    public sealed class TaskAddedEventArgs : TaskExecutorEventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public TaskAddedEventArgs(ITaskExecutor executor, ITask task)
            : base(executor, false)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            Task = task;
        }

        #region Properties

        /// <summary>
        /// Gets the task associated with last action.
        /// </summary>
        public ITask Task
        {
            get;
            private set;
        }

        #endregion
    }
}