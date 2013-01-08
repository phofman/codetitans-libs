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
    /// Advanced comparer for bayeux channels.
    /// It allowes some of the segments be identical if source pattern is defined as '*'.
    /// </summary>
    internal sealed class WildcardChannelProcessor : ChannelProcessor
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public WildcardChannelProcessor(string channel, string[] segments, IChannelHandler handler, object state)
            : base(channel, segments, handler, state)
        {
        }

        /// <summary>
        /// Checks if given channel matches the one described by this processor.
        /// </summary>
        public override bool Matches(string channel, string[] channelSegments)
        {
            if (Segments == null || Segments.Length != channelSegments.Length)
                return false;

            for (int i = 0; i < Segments.Length; i++)
                if (!(Segments[i].Length == 1 && Segments[i][0] == '*'))
                    if (string.CompareOrdinal(Segments[i], channelSegments[i]) != 0)
                        return false;

            return true;
        }
    }
}
