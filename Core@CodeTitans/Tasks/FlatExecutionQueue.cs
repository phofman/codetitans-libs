#region License
/*
    Copyright (c) 2010, Pawe³ Hofman (CodeTitans)
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
using CodeTitans.Diagnostics;

namespace CodeTitans.Core.Tasks
{
    /// <summary>
    ///  Class implementing task execution in the main thread.
    ///  It uses monitoring of status changes and executes next task
    ///  inside the same thread as the previous one finished.
    /// </summary>
    public class FlatExecutionQueue : ITaskExecutor
    {
        private readonly List<ITask> _queue;
        private bool _started;
        private int _count;
        private ITask _currentTask;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public FlatExecutionQueue(string name)
        {
            _queue = new List<ITask>();
            Name = name;
        }

        #region ITaskExecutor implementation

        /// <summary>
        /// Event fired each time each time a new task is added and scheduled for execution.
        /// </summary>
        public event EventHandler<TaskAddedEventArgs> TaskAdded;

        /// <summary>
        /// Event fired each time all tasks in the internal queue has been finished and there is nothing to do.
        /// </summary>
        public event EventHandler<TaskExecutorEventArgs> QueueEmpty;

        /// <summary>
        /// Event fired each time a task has finished with unhandled exception.
        /// </summary>
        public event EventHandler<TaskExceptionedEventArgs> UnhandledException;

        /// <summary>
        /// Queues new task for execution.
        /// If the executor is already 'started', the task is executed immediatelly.
        /// </summary>
        public void Add(ITask task)
        {
            InternalAdd(task);
            ExecuteTask();
        }

        private void InternalAdd(ITask task)
        {
            if (task.Dependencies != null)
                foreach (ITask dependency in task.Dependencies)
                {
                    if (dependency.Parent != task)
                        throw new ArgumentOutOfRangeException("task", "Parent value set invalid, probably task contained as dependency for several other tasks");

                    InternalAdd(dependency);
                }

            // remember the task for further execution:
            task.StatusChanged += TaskStatusChanged;
            _queue.Add(task);
            _count = _queue.Count;

            Event.Invoke(TaskAdded, this, new TaskAddedEventArgs(this, task));
        }

        /// <summary>
        /// Queues new task for execution.
        /// If the executor is already 'started', the task is executed immediatelly.
        /// </summary>
        public void Add(Action<ITaskExecutor> task)
        {
            Add(new ActionTask(task));
        }

        /// <summary>
        /// Queues new task for execution.
        /// If the executor is already 'started', the task is executed immediatelly.
        /// </summary>
        public void AddRange(IEnumerable<ITask> tasks)
        {
            foreach (var t in tasks)
                Add(t);
        }

        /// <summary>
        /// Cancels current task.
        /// </summary>
        public void Cancel(object reason)
        {
            Cancel(Current, reason);
        }

        /// <summary>
        /// Cancels selected task.
        /// </summary>
        public void Cancel(ITask task, object reason)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            task.Cancel(reason);
            RemoveTask(task);
        }

        /// <summary>
        /// Cancels all tasks.
        /// </summary>
        public void CancelAll(object reason)
        {
            ITask[] tasks = _queue.ToArray();
            _queue.Clear();
            _count = 0;
            _currentTask = null;

            // cancel all tasks in the reverse order, so that new tasks
            // are not started as the currently executed one is finished with cancel:
            if (tasks.Length > 0)
            {
                for (int i = tasks.Length - 1; i >= 0; i--)
                {
                    ITask task = tasks[i];
                    task.StatusChanged -= TaskStatusChanged;
                    task.Cancel(reason);
                }

                Event.Invoke(QueueEmpty, this, new TaskExecutorEventArgs(this, !_started));
            }
        }

        /// <summary>
        /// Starts processing of stored tasks.
        /// </summary>
        public void Start()
        {
            _started = true;
            ExecuteTask();
        }

        /// <summary>
        /// Stops processing of stored tasks.
        /// </summary>
        public void Stop(bool waitUntilFinished)
        {
            Stop(waitUntilFinished, null);
        }

        /// <summary>
        /// Stops processing of stored tasks.
        /// </summary>
        public void Stop(bool waitUntilFinished, object reason)
        {
            _started = false;
            CancelAll(reason);
        }

        /// <summary>
        /// Gets or sets the name of the executor.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the indication if executor is actively processing tasks.
        /// </summary>
        public bool IsStarted
        {
            get { return _started; }
        }

        /// <summary>
        /// Gets the current task that is being executed.
        /// </summary>
        public ITask Current
        {
            get { return _currentTask; }
        }

        /// <summary>
        /// Gets the enumerable collection of tasks.
        /// </summary>
        public IEnumerable<ITask> Tasks
        {
            get { return _queue.ToArray(); }
        }

        /// <summary>
        /// Gets the number of stored tasks.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        #endregion

        private void ExecuteTask()
        {
            // don't execute, if the ExecutionQueue is not marked as running:
            if (!_started)
                return;

            // select an object to execute:
            if (_queue.Count > 0)
            {
                ITask task = _queue[0];

                // and if it's not already running, start it:
                if (task.Status == TaskStatus.Waiting)
                    ExecuteTask(task);
            }
            else
            {
                if (_currentTask != null)
                {
                    _currentTask = null;
                    DebugLog.WriteCoreLine("No task to execute in FlatExecutionQueue.");
                    Event.Invoke(QueueEmpty, this, new TaskExecutorEventArgs(this, !_started));
                }
            }
        }

        private void ExecuteTask(ITask task)
        {
            try
            {
                DebugLog.WriteCoreLine("Executing task: '" + task.Name + "'");
                _currentTask = task;
                task.Execute(this);
            }
            catch (Exception ex)
            {
                var e = new TaskExceptionedEventArgs(this, task, ex);

                Event.Invoke(UnhandledException, this, e);

                // if the exception is not handled by external parties:
                if (!e.Handled)
                {
                    DebugLog.WriteCoreException(ex);

#if DEBUG
                    // this is not desired situation, so we hold the whole program!
                    Debugger.Break();
#endif
                }
            }
        }

        private void TaskStatusChanged(object sender, TaskStatusChangedEventArgs e)
        {
            // when task is finished, go to processing of the next one:
            if (e.IsFinished)
            {
                RemoveTask(e.Task);
                ExecuteTask();
            }
        }

        private void RemoveTask(ITask task)
        {
            task.StatusChanged -= TaskStatusChanged;
            _queue.Remove(task);
            _count = _queue.Count;
        }
    }
}

