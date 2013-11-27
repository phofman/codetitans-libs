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
    /// Enumeration describing modes of the registered service.
    /// </summary>
    public enum ServiceMode
    {
        /// <summary>
        /// The same instance as passed during the registration will be used each time.
        /// </summary>
        Singleton,
        /// <summary>
        /// New instance of the service will be created for each request.
        /// </summary>
        Clone
    }
}