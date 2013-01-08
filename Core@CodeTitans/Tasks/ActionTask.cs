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
    /// Task that executes a given action method.
    /// </summary>
    public sealed class ActionTask : MethodCallTask
    {
        private readonly Action<ITaskExecutor> _action;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ActionTask(Action<ITaskExecutor> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            _action = action;
        }

        /// <summary>
        /// Execute specified action.
        /// </summary>
        public override void Execute(ITaskExecutor executionQueue)
        {
            _action(executionQueue);
        }
    }
}
