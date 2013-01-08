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
    /// Reconnection action specified by Bayeux server to its clients.
    /// </summary>
    public enum BayeuxAdviceReconnectType
    {
        /// <summary>
        /// Advice saying 'do nothing' and 'don't even try to connect'.
        /// </summary>
        None,
        /// <summary>
        /// Advice saying 'try to redo handshake'.
        /// </summary>
        Handshake,
        /// <summary>
        /// Advice saying 'try again that operation'.
        /// </summary>
        Retry
    }
}
