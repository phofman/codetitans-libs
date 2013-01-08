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

namespace CodeTitans.Bayeux.Channels.Processors
{
    /// <summary>
    /// Basic channel comparer. Channels must be equal full, char by char.
    /// </summary>
    internal class BasicChannelProcessor : ChannelProcessor
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BasicChannelProcessor(string channel, string[] segments, IChannelHandler handler, object state)
            : base(channel, segments, handler, state)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BasicChannelProcessor(IChannelHandler handler, object state)
            : base(handler, state)
        {
        }

        /// <summary>
        /// Checks if given channel matches the one described by this processor.
        /// </summary>
        public override bool Matches(string channel, string[] channelSegments)
        {
            // always match, when no given channel specified (this is only possible via one constructor):
            if (string.IsNullOrEmpty(Channel))
                return true;

            if (Segments == null || Segments.Length != channelSegments.Length)
                return false;

            if (Channel.Length != channel.Length)
                return false;

            return string.CompareOrdinal(Channel, channel) == 0;
        }
    }
}
