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

namespace CodeTitans.Services
{
    /// <summary>
    /// Interface for defining new kinds of services and assigning a singleton service objects for them.
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Registers a given object as a singleton service by using all of its implemented interfaces.
        /// </summary>
        void Register(object service);
    }
}