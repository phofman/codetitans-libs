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

namespace CodeTitans.Bayeux.Channels.Processors
{
    /// <summary>
    /// Default channel comparer. Defines API for basic functionality, ie. validation, matching and notifications.
    /// </summary>
    internal abstract class ChannelProcessor
    {
        /// <summary>
        /// Special marks that are allowed in channel name.
        /// </summary>
        public const string MetaMarks = "-_!~(){}$@/*";

        /// <summary>
        /// Init constructor.
        /// </summary>
        protected ChannelProcessor(string channel, string[] segments, IChannelHandler handler, object state)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");
            if (!IsValid(channel))
                throw new ArgumentException("Channel seems to be invalid due to standard validation checks", "channel");

            Channel = channel;
            Segments = segments;
            Handler = handler;
            State = state;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        protected ChannelProcessor(IChannelHandler handler, object state)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            Handler = handler;
            State = state;
        }

        #region Properties

        /// <summary>
        /// Gets the channel current handler is monitoring for.
        /// </summary>
        public string Channel
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the channel's segments current handler is looking for.
        /// </summary>
        public string[] Segments
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the handler called when channel is matching the expected one.
        /// </summary>
        public IChannelHandler Handler
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the any object that represents any user-state.
        /// </summary>
        public object State
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Returns 'true' if given text is a valid Bayeux channel name.
        /// It should consist of number letters or digits separated by '/'.
        /// </summary>
        public static bool IsValid(string channel)
        {
            if (string.IsNullOrEmpty(channel))
                return false;

            foreach (char c in channel)
            {
                if (!char.IsLetterOrDigit(c) && MetaMarks.IndexOf(c) == -1)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if given channel matches the one described by this processor.
        /// </summary>
        public abstract bool Matches(string channel, string[] channelSegments);

        /// <summary>
        /// Notify external handler, that found a matching channel.
        /// </summary>
        public void Notify(string channel, string[] channelSegments, object data)
        {
            Handler.Handle(channel, channelSegments, data, State);
        }
    }
}
