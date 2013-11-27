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
    /// Interface implemented by types of tasks processed by execution queue manager.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Event fired when current task has changed it state.
        /// It is mostly important to notify listeners that task has finished its execution.
        /// </summary>
        event EventHandler<TaskStatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Gets an indication if a task is assumed finished,
        /// when Execute() method if performed or waiting for TaskFinished event
        /// will notify that.
        /// </summary>
        bool IsConcurrent
        { get; }

        /// <summary>
        /// Gets the current status of the task.
        /// </summary>
        TaskStatus Status
        { get; }

        /// <summary>
        /// String description of current task status.
        /// </summary>
        string StatusDescription
        { get; }

        /// <summary>
        /// Gets or sets the reference of the parent task.
        /// </summary>
        ITask Parent
        { get; set; }

        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        string Name
        { get; }

        /// <summary>
        /// Gets or sets a custom object associated with this task.
        /// </summary>
        object Tag
        { get; set; }

        /// <summary>
        /// Gets the enumerable collection of other dependant tasks.
        /// </summary>
        IEnumerable<ITask> Dependencies
        { get; }

        /// <summary>
        /// Adds new task as dependency of current one.
        /// It means execution queue manager will call it first.
        /// </summary>
        void AddDependency(ITask task);

        /// <summary>
        /// Removes given task from dependency list.
        /// </summary>
        void RemoveDependency(ITask task);

        /// <summary>
        /// Method called when given task should perform it's task.
        /// </summary>
        void Execute(ITaskExecutor executionQueue);

        /// <summary>
        /// Method called when given task is cancelled with ability to pass reason parameters.
        /// </summary>
        void Cancel(object reason);
    }
}
