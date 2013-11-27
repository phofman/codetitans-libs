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
    /// Generic Bayeux connect request.
    /// </summary>
    internal sealed class ConnectRequest : BayeuxRequest
    {
        /// <summary>
        /// Channel name.
        /// </summary>
        public const string MetaChannel = "/meta/connect";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ConnectRequest(string clientID, BayeuxConnectionTypes connectionType, IJSonWritable data, IJSonWritable ext)
            : base (MetaChannel, data, ext)
        {
            if (string.IsNullOrEmpty(clientID))
                throw new ArgumentException("ClientID can not be empty", "clientID");

            ClientID = clientID;
            ConnectionType = connectionType;
        }

        #region Properties

        /// <summary>
        /// Gets the connection type.
        /// </summary>
        public BayeuxConnectionTypes ConnectionType
        { get; private set; }

        #endregion

        protected override void WriteOptionalFields(IJSonWriter output)
        {
            output.WriteMember("connectionType", BayeuxConnectionTypesHelper.ToString(ConnectionType));
        }
    }
}
