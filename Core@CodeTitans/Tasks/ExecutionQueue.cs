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
using System.Threading;
using CodeTitans.Diagnostics;

namespace CodeTitans.Core.Tasks
{
#if !WINDOWS_STORE
    /// <summary>
    /// Class implementing task execution in a dedicated thread.
    /// All of them are processed sequentially and dependencies will always be finished
    /// first before performing the owner task.
    /// </summary>
    public class ExecutionQueue : ITaskExecutor
    {
        private readonly object _sync;
        private readonly AutoResetEvent _startEvent;
        private readonly List<ITask> _queue;
        private readonly Dictionary<int, ITask> _shadowQueue;
        private readonly bool _isBackground;
        private int _count;
        private volatile bool _stopExecution;
        private ITask _currentTask;
        private Thread _backgroundThread;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ExecutionQueue(string name, bool isBackground)
        {
            _sync = new object();
            _startEvent = new AutoResetEvent(false);
            _queue = new List<ITask>();
            _shadowQueue = new Dictionary<int, ITask>();
            _isBackground = isBackground;

            Name = name;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ExecutionQueue(string name)
            : this(name, false)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ExecutionQueue()
            : this(null, false)
        {
        }

        #region ITaskExecutor Members

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
        /// Gets or sets the name of the executor.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the indication if executor is actively processing tasks.
        /// </summary>
        public bool IsStarted
        {
            get { return _backgroundThread != null; }
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
            get
            {
                lock (_sync)
                {
                    return _queue.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets the number of stored tasks.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Queues new task for execution.
        /// </summary>
        public void Add(ITask task)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            InternalAdd(task, true);
            _startEvent.Set();
        }

        /// <summary>
        /// Queues new task for execution.
        /// </summary>
        public void Add(Action<ITaskExecutor> task)
        {
            Add(new ActionTask(task));
        }

        /// <summary>
        /// Queues a collection of tasks for execution.
        /// </summary>
        public void AddRange(IEnumerable<ITask> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException("tasks");

            foreach (ITask task in tasks)
            {
                InternalAdd(task, true);
            }
            _startEvent.Set();
        }

        private bool InternalContains(ITask task)
        {
            return _shadowQueue.ContainsKey(task.GetHashCode());
        }

        private void InternalAdd(ITask task, bool throwIfContains)
        {
            if (task.Dependencies != null)
                foreach (ITask dependency in task.Dependencies)
                {
                    if (dependency.Parent != task)
                        throw new ArgumentOutOfRangeException("task", "Parent value set invalid, probably task contained as dependency for several other tasks");

                    InternalAdd(dependency, false);
                }

            lock (_sync)
            {
                if (InternalContains(task))
                {
                    if (throwIfContains)
                        throw new ArgumentException("Task already added");
                    return;
                }

                // remember the task for further execution:
                task.StatusChanged += TaskStatusChanged;
                _queue.Add(task);
                _shadowQueue.Add(task.GetHashCode(), task);
                _count = _queue.Count;
            }

            Event.Invoke(TaskAdded, this, new TaskAddedEventArgs(this, task));
        }

        /// <summary>
        /// Cancels current task.
        /// </summary>
        public void Cancel(object reason)
        {
            if (_currentTask != null)
            {
                _currentTask.Cancel(reason);
                _currentTask.StatusChanged -= TaskStatusChanged;
            }
            ClearCurrent();
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

        private void RemoveTask(ITask task)
        {
            task.StatusChanged -= TaskStatusChanged;
            if (task == _currentTask)
            {
                ClearCurrent();
            }
            else
            {
                lock (_sync)
                {
                    if (InternalContains(task))
                    {
                        _queue.Remove(task);
                        _shadowQueue.Remove(task.GetHashCode());
                        _count = _queue.Count;
                    }
                }
            }
        }

        /// <summary>
        /// Cancels all tasks.
        /// </summary>
        public void CancelAll(object reason)
        {
            ITask[] tasks;

            lock (_sync)
            {
                tasks = _queue.ToArray();
                _queue.Clear();
                _shadowQueue.Clear();
                _count = 0;
                _currentTask = null;
            }

            // cancel all tasks in the reverse order, so that new tasks
            // are not started as the currently executed one is finished with cancel:
            if (tasks.Length > 0)
                for (int i = tasks.Length - 1; i >= 0; i--)
                {
                    ITask task = tasks[i];
                    task.StatusChanged -= TaskStatusChanged;
                    task.Cancel(reason);
                }

            lock (_sync)
            {
                _startEvent.Set();
            }
        }

        private void ClearCurrent()
        {
            lock (_sync)
            {
                if (_currentTask != null)
                {
                    _queue.Remove(_currentTask);
                    _shadowQueue.Remove(_currentTask.GetHashCode());
                    _count = _queue.Count;
                    _currentTask = null;
                    _startEvent.Set();
                }
            }
        }

        /// <summary>
        /// Starts processing of stored tasks.
        /// </summary>
        public void Start()
        {
            if (_backgroundThread == null)
            {
                _stopExecution = false;
                _backgroundThread = new Thread(ExecuteTasks);
                _backgroundThread.Name = Name;
                _backgroundThread.IsBackground = _isBackground;
                _backgroundThread.Start();
                _startEvent.Set();
            }
        }

        /// <summary>
        /// Stops processing of stored tasks.
        /// It can optionally wait until all tasks are done.
        /// </summary>
        public void Stop(bool waitUntilFinished)
        {
            Stop(waitUntilFinished, null);
        }

        /// <summary>
        /// Stops processing of stored tasks.
        /// It can optionally wait until all tasks are done.
        /// </summary>
        public void Stop(bool waitUntilFinished, object reason)
        {
            if (_backgroundThread != null)
            {
                Thread thread = _backgroundThread;

                if (waitUntilFinished)
                {
                    _stopExecution = true;
                    _startEvent.Set();
                    thread.Join();
                    _backgroundThread = null;
                }
                else
                {
                    CancelAll(reason);

                    _backgroundThread = null;
                    _stopExecution = true;
                    _startEvent.Set();
                }
            }
        }

        private void ExecuteTasks()
        {
            try
            {
                bool executedTaskInPrevLoop = false;

                while (true)
                {
                    if (executedTaskInPrevLoop && QueueEmpty != null && _count == 0)
                    {
                        executedTaskInPrevLoop = false;
                        Event.Invoke(QueueEmpty, this, new TaskExecutorEventArgs(this, false));
                    }

                    // wait until new task has been added into the queue:
                    if (_startEvent.WaitOne())
                    {
                        // stop processing, when externally requested:
                        if (_stopExecution && _backgroundThread == null)
                            break;
                        if (_stopExecution && _count == 0)
                            break;

                        ITask task = GetNextTask();

                        if (task != null)
                        {
                            if (task.IsConcurrent)
                            {
                                if (task != _currentTask)
                                {
                                    // execute the concurrent task:
                                    ExecuteTask(task);
                                    executedTaskInPrevLoop = true;
                                }
                            }
                            else
                            {
                                // to improve performance and number of waitings
                                // on an event in this thread, execute all non-concurrent
                                // tasks and first that is concurrent, to have something to wait for next:
                                while (task != null)
                                {
                                    ExecuteTask(task);
                                    executedTaskInPrevLoop = true;

                                    if (task.IsConcurrent)
                                        break;

                                    RemoveTask(task);
                                    task = GetNextTask();
                                }

                                if (task == null)
                                    _currentTask = null;
                            }
                        }
                        else
                        {
                            DebugLog.WriteCoreLine("No task to execute in ExecutionQueue.");
                        }
                    }
                    else
                    {
                        // error while accessing event, better close:
                        break;
                    }
                }

                if (QueueEmpty != null && _stopExecution)
                    Event.Invoke(QueueEmpty, this, new TaskExecutorEventArgs(this, true));
            }
            catch (ThreadAbortException)
            {
                // Ignore this single exception, as this might happen, when process is killed
                // and we don't want to finish the application with an exception instead...
            }
        }

        /// <summary>
        /// Gets the next task to execute.
        /// </summary>
        private ITask GetNextTask()
        {
            lock (_sync)
            {
                if (_currentTask != null)
                    return _currentTask;

                if (_queue.Count > 0)
                    return _queue[0];
            }

            return null;
        }

        private void ExecuteTask(ITask task)
        {
            try
            {
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
                _startEvent.Set();
            }
        }

        #endregion
    }
#endif
}
