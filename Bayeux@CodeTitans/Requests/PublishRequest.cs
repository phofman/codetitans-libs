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
    /// Generic Bayeux publish request.
    /// </summary>
    internal sealed class PublishRequest : BayeuxRequest
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public PublishRequest(string clientID, string channel, IJSonWritable eventData, IJSonWritable data, IJSonWritable ext)
            : base (channel, data, ext)
        {
            if (!BayeuxChannel.IsValid(channel))
                throw new ArgumentException("Channel failed a standard validation", "channel");

            ClientID = clientID;
            EventData = eventData;
        }

        #region Properties

        /// <summary>
        /// Gets the data associated with this request.
        /// </summary>
        public IJSonWritable EventData
        { get; private set; }

        #endregion

        #region Overrides

        protected override void WriteOptionalFields(IJSonWriter output)
        {
            output.WriteMember("data");
            output.Write(EventData);
        }

        #endregion
    }
}
