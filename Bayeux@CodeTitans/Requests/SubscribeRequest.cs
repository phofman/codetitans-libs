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
using CodeTitans.JSon;

namespace CodeTitans.Bayeux.Requests
{
    /// <summary>
    /// Generic Bayeux subscribe request.
    /// </summary>
    internal sealed class SubscribeRequest : BayeuxRequest
    {
        /// <summary>
        /// Channel name.
        /// </summary>
        public const string MetaChannel = "/meta/subscribe";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public SubscribeRequest(string clientID, string subscriptionChannel, IJSonWritable data, IJSonWritable ext)
            : base(MetaChannel, data, ext)
        {
            if (!BayeuxChannel.IsValid(subscriptionChannel))
                throw new ArgumentException("SubscriptionChannel failed a standard validation", "subscriptionChannel");

            ClientID = clientID;
            SubscriptionChannel = subscriptionChannel;
        }

        #region Properties

        /// <summary>
        /// Gets the name of the channel, where to subscribe.
        /// </summary>
        public string SubscriptionChannel
        { get; private set; }

        #endregion

        #region Overrides

        protected override void WriteOptionalFields(CodeTitans.JSon.IJSonWriter output)
        {
            output.WriteMember("subscription", SubscriptionChannel);
        }

        #endregion
    }
}
