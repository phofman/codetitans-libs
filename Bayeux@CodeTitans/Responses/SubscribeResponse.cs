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

using CodeTitans.JSon;

namespace CodeTitans.Bayeux.Responses
{
    /// <summary>
    /// Generic subscription response.
    /// </summary>
    internal sealed class SubscribeResponse : BayeuxResponse
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public SubscribeResponse(IJSonObject data)
            : base (data)
        {
        }

        #region Properties

        /// <summary>
        /// Gets the name of the channel, where to subscribe.
        /// </summary>
        public string SubscriptionChannel
        { get; private set; }

        #endregion

        #region Overrides

        protected override void ReadOptionalFields(IJSonObject input)
        {
            // reset field values:
            SubscriptionChannel = null;

            if (input.Contains("subscription"))
                SubscriptionChannel = input["subscription"].StringValue;
        }

        protected override void WriteOptionalFields(IJSonWriter output)
        {
            output.WriteMember("subscription", SubscriptionChannel);
        }

        #endregion
    }
}
