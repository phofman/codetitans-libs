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
    /// Arguments passed when task is notifying about its finish status.
    /// </summary>
    public class TaskStatusChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public TaskStatusChangedEventArgs(ITask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            Task = task;
        }

        #region Properties

        /// <summary>
        /// Gets the associated task.
        /// </summary>
        public ITask Task
        { get; private set; }

        /// <summary>
        /// Gets the task status.
        /// </summary>
        public TaskStatus Status
        {
            get { return Task.Status; }
        }

        /// <summary>
        /// String description of current task status.
        /// </summary>
        public string StatusDescription
        {
            get { return Task.StatusDescription; }
        }

        /// <summary>
        /// Gets an indication if task is finished.
        /// </summary>
        public bool IsFinished
        {
            get { return Task.Status == TaskStatus.FinishedWithSuccess || Task.Status == TaskStatus.FinishedWithFailure || Task.Status == TaskStatus.FinishedWithCancel; }
        }

        /// <summary>
        /// Gets an indication if task waits for execution or is already running.
        /// </summary>
        public bool IsInProgress
        {
            get { return Task.Status == TaskStatus.Waiting || Task.Status == TaskStatus.Executing || Task.Status == TaskStatus.Cancelling; }
        }

        #endregion
    }
}
