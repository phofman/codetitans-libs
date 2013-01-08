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
using CodeTitans.Bayeux.Channels.Processors;

namespace CodeTitans.Bayeux.Channels
{
    /// <summary>
    /// Class responsible for dispatching incomming bayeux channels for dedicated handlers.
    /// Handler is registered for a signle channel using a 'strict' matching
    /// or for multiple channels if using wildcards '*' or '**'.
    /// </summary>
    public sealed class ChannelDispatcher
    {
        private readonly List<ChannelProcessor> _processors;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ChannelDispatcher()
        {
            _processors = new List<ChannelProcessor>();
        }

        #region Properties

        /// <summary>
        /// Gets or sets the default channel processor.
        /// </summary>
        private ChannelProcessor DefaultProcessor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the default handler for channels that didn't match any other criteria.
        /// </summary>
        public IChannelHandler DefaultHandler
        {
            get { return DefaultProcessor.Handler; }
        }

        #endregion

        /// <summary>
        /// Registers new handler to notify, when processing expected channel.
        /// </summary>
        public void Register(string channel, IChannelHandler handler)
        {
            Register(channel, handler, null);
        }

        /// <summary>
        /// Registers new handler to notify, when processing expected channel.
        /// </summary>
        public void Register(string channel, IChannelHandler handler, object state)
        {
            if (string.IsNullOrEmpty(channel))
            {
                RegisterDefault(handler);
            }

            if (handler == null)
                throw new ArgumentNullException("handler");

            _processors.Add(CreateNewProcessor(channel, handler, state));
        }

        /// <summary>
        /// Registers the default handler.
        /// If during processing a given channel doesn't fit for any handler, this one will get called.
        /// </summary>
        public void RegisterDefault(IChannelHandler handler)
        {
            RegisterDefault(handler, null);
        }

        /// <summary>
        /// Registers the default handler.
        /// If during processing a given channel doesn't fit for any handler, this one will get called.
        /// </summary>
        public void RegisterDefault(IChannelHandler handler, object state)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            DefaultProcessor = new BasicChannelProcessor(handler, state);
        }

        private ChannelProcessor CreateNewProcessor(string channel, IChannelHandler handler, object state)
        {
            var segments = channel.Split('/');
            int globalWildcardIndex = channel.IndexOf("/**");
            int singleWildcardIndex = channel.IndexOf('*');

            if (globalWildcardIndex >= 0)
            {
                if (globalWildcardIndex != channel.Length - 3)
                    throw new NotSupportedException("Global wildcards are only supported at the end of the channel");
                return new GlobalWildcardChannelProcessor(channel, segments, handler, state);
            }

            if (singleWildcardIndex >= 0)
                return new WildcardChannelProcessor(channel, segments, handler, state);

            return new BasicChannelProcessor(channel, segments, handler, state);
        }

        /// <summary>
        /// Handles incomming notification from given channel.
        /// Internal collection of processors is looked for the best matching one and this one will get right to proceed with notifiation.
        /// </summary>
        public bool Handle(string channel, object data)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException("channel");

            var segments = channel.Split('/');

            foreach (var processor in _processors)
                if (processor.Matches(channel, segments))
                {
                    processor.Notify(channel, segments, data);
                    return true;
                }

            if (DefaultProcessor != null)
                DefaultProcessor.Notify(channel, segments, data);

            return false;
        }
    }
}
