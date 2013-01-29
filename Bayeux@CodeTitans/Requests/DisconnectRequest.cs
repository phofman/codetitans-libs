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

namespace CodeTitans.Bayeux.Requests
{
    /// <summary>
    /// Generic Bayeux disconnect request.
    /// </summary>
    internal sealed class DisconnectRequest : BayeuxRequest
    {
        /// <summary>
        /// Channel name.
        /// </summary>
        public const string MetaChannel = "/meta/disconnect";

        /// <summary>
        /// Init constructor.
        /// </summary>
        public DisconnectRequest(string clientID, IJSonWritable data, IJSonWritable ext)
            : base (MetaChannel, data, ext)
        {
            ClientID = clientID;
        }
    }
}
