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

namespace CodeTitans.Bayeux.Channels
{
    /// <summary>
    /// Interface defining handler of the channel.
    /// </summary>
    public interface IChannelHandler
    {
        /// <summary>
        /// Method called, when incomming channel is matching given criteria and should be now handled.
        /// </summary>
        void Handle(string channel, string[] segments, object data, object state);
    }
}
