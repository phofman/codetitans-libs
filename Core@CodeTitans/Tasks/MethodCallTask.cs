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
using System.Threading;

namespace CodeTitans.Core.Tasks
{
    /// <summary>
    /// Abstract task that performs a method.
    /// </summary>
    public abstract class MethodCallTask : ITask
    {
        private TaskStatus _status;
        private IList<ITask> _dependencies;

        /// <summary>
        /// Event fired when current task has changed it state.
        /// It is mostly important to notify listeners that task has finished its execution.        /// </summary>
        public event EventHandler<TaskStatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MethodCallTask()
        {
            _status = TaskStatus.Waiting;
        }

        /// <summary>
        /// Gets an indication if a task is assumed finished,
        /// when Execute() method if performed or waiting for TaskFinished event
        /// will notify that.
        /// </summary>
        public bool IsConcurrent
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the current status of the task.
        /// </summary>
        public TaskStatus Status
        {
            get { return _status; }
            private set
            {
                if (_status != value)
                {
#if !DISABLE_INTERLOCKED
                    EventHandler<TaskStatusChangedEventArgs> handler = Interlocked.CompareExchange(ref StatusChanged, null, null);
#else
                    EventHandler<TaskStatusChangedEventArgs> handler = StatusChanged;
#endif
                    _status = value;
                    if (handler != null)
                        handler(this, new TaskStatusChangedEventArgs(this));
                }
            }
        }

        /// <summary>
        /// String description of current task status.
        /// </summary>
        public string StatusDescription
        {
            get { return Status.ToString(); }
        }

        /// <summary>
        /// Gets or sets the reference of the parent task.
        /// </summary>
        public ITask Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of this task.
        /// </summary>
        public string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets or sets a custom object associated with this task.
        /// </summary>
        public object Tag
        { get; set; }

        /// <summary>
        /// Gets the enumerable collection of other dependant tasks.
        /// </summary>
        public IEnumerable<ITask> Dependencies
        {
            get { return _dependencies; }
        }

        /// <summary>
        /// Adds new task as dependency of current one.
        /// It means execution queue manager will call it first.
        /// </summary>
        public void AddDependency(ITask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            if (_dependencies == null)
                _dependencies = new List<ITask>();

            if (!_dependencies.Contains(task))
            {
                task.Parent = this;
                _dependencies.Add(task);
            }
        }

        /// <summary>
        /// Removes given task from dependency list.
        /// </summary>
        public void RemoveDependency(ITask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            if (_dependencies != null)
            {
                _dependencies.Remove(task);
            }
        }

        /// <summary>
        /// Method called when given task should perform it's task.
        /// </summary>
        public abstract void Execute(ITaskExecutor executionQueue);

        /// <summary>
        /// Method called when given task is cancelled with ability to pass reason parameters.
        /// </summary>
        public void Cancel(object reason)
        {
            Status = TaskStatus.FinishedWithCancel;

            // nothing to do, as this is a non-concurrent method wrapper...
        }
    }
}
