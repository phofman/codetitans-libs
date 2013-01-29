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

namespace CodeTitans.Core.Tasks
{
    /// <summary>
    /// Statuses of ITask.
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// State of newly created task. It is waiting for execution.
        /// </summary>
        Waiting,
        /// <summary>
        /// Task is currently being executed.
        /// </summary>
        Executing,
        /// <summary>
        /// Task is currently being cancelled.
        /// </summary>
        Cancelling,
        /// <summary>
        /// Task is finished with success.
        /// </summary>
        FinishedWithSuccess,
        /// <summary>
        /// Task is finished with failure.
        /// </summary>
        FinishedWithFailure,
        /// <summary>
        /// Task is finished by cancel.
        /// </summary>
        FinishedWithCancel
    }
}
