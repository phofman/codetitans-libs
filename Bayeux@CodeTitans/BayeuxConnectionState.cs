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

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Type describing current state of the Bayeux connection.
    /// </summary>
    public enum BayeuxConnectionState
    {
        /// <summary>
        /// Connection is not performed with the remote server.
        /// </summary>
        Disconnected,
        /// <summary>
        /// Connection is establishing.
        /// </summary>
        Connecting,
        /// <summary>
        /// Connection established successfully.
        /// </summary>
        Connected
    }
}
