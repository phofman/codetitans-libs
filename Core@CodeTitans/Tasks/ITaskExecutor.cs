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

namespace CodeTitans.Core.Tasks
{
    /// <summary>
    /// Interface defining basic behaviour of 
    /// </summary>
    public interface ITaskExecutor
    {
        /// <summary>
        /// Event fired each time each time a new task is added and scheduled for execution.
        /// </summary>
        event EventHandler<TaskAddedEventArgs> TaskAdded;

        /// <summary>
        /// Event fired each time all tasks in the internal queue has been finished and there is nothing to do.
        /// </summary>
        event EventHandler<TaskExecutorEventArgs> QueueEmpty;

        /// <summary>
        /// Event fired each time a task has finished with unhandled exception.
        /// </summary>
        event EventHandler<TaskExceptionedEventArgs> UnhandledException;

        /// <summary>
        /// Gets the name of the executor.
        /// </summary>
        string Name
        { get; }

        /// <summary>
        /// Gets the indication if executor is actively processing tasks.
        /// </summary>
        bool IsStarted
        { get; }

        /// <summary>
        /// Gets the current task that is being executed.
        /// </summary>
        ITask Current
        { get; }

        /// <summary>
        /// Gets the enumerable collection of tasks.
        /// </summary>
        IEnumerable<ITask> Tasks
        { get; }

        /// <summary>
        /// Gets the number of stored tasks.
        /// </summary>
        int Count
        { get; }

        /// <summary>
        /// Queues new task for execution.
        /// </summary>
        void Add(ITask task);

        /// <summary>
        /// Queues new task for execution.
        /// </summary>
        void Add(Action<ITaskExecutor> task);

        /// <summary>
        /// Queues a collection of tasks for execution.
        /// </summary>
        void AddRange(IEnumerable<ITask> tasks);

        /// <summary>
        /// Cancels current task.
        /// </summary>
        void Cancel(object reason);

        /// <summary>
        /// Cancels selected task.
        /// </summary>
        void Cancel(ITask task, object reason);

        /// <summary>
        /// Cancels all tasks.
        /// </summary>
        void CancelAll(object reason);

        /// <summary>
        /// Starts processing of stored tasks.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops processing of stored tasks.
        /// It can optionally wait until all tasks are done.
        /// </summary>
        void Stop(bool waitUntilFinished);

        /// <summary>
        /// Stops processing of stored tasks.
        /// It can optionally wait until all tasks are done.
        /// </summary>
        void Stop(bool waitUntilFinished, object reason);
    }
}
