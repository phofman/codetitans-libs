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
    /// Generic Bayeux handshake request.
    /// </summary>
    internal sealed class HandshakeRequest : BayeuxRequest
    {
        /// <summary>
        /// Channel name.
        /// </summary>
        public const string MetaChannel = "/meta/handshake";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public HandshakeRequest(BayeuxConnectionTypes supportedConnectionTypes, IJSonWritable data, IJSonWritable ext)
            : base (MetaChannel, data, ext)
        {
            Version = new Version(1, 0);
            MinimumVersion = Version;
            SupportedConnectionTypes = supportedConnectionTypes;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public HandshakeRequest(IJSonWritable data, IJSonWritable extData)
            : this(BayeuxConnectionTypes.LongPolling | BayeuxConnectionTypes.CallbackPolling | BayeuxConnectionTypes.Iframe, data, extData)
        {
        }

        #region Properties

        /// <summary>
        /// Type of the connection.
        /// </summary>
        public BayeuxConnectionTypes SupportedConnectionTypes
        { get; set; }

        /// <summary>
        /// Version of the Bayeux protocol.
        /// </summary>
        public Version Version
        { get; set; }

        /// <summary>
        /// Minimum version of the Bayeux protocol expected.
        /// </summary>
        public Version MinimumVersion
        { get; set; }

        #endregion

        #region Overrides

        protected override void WriteOptionalFields(IJSonWriter output)
        {
            // create basic JSON representation of Bayeux message:
            // {
            //   ... - parent inserted fields
            //  supportedConnectionTypes: "xxx",
            //  version: "1.0",
            //  minimumVersion: "1.0",
            //  ext: "xxx"
            // }

            if (Version != null)
                output.WriteMember("version", Version.ToString());
            if (MinimumVersion != null)
                output.WriteMember("minimumVersion", MinimumVersion.ToString());

            output.WriteMember("supportedConnectionTypes");
            output.WriteArrayBegin();
            foreach (string type in BayeuxConnectionTypesHelper.ToCollection(SupportedConnectionTypes))
                output.WriteValue(type);
            output.WriteArrayEnd();
        }

        #endregion
    }
}
