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
using CodeTitans.Bayeux.Requests;
using CodeTitans.Bayeux.Responses;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Exception thrown by <see cref="BayeuxConnection"/> class.
    /// </summary>
    [Serializable]
    public sealed class BayeuxException : Exception
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxException()
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxException(string message, BayeuxRequest request, BayeuxResponse response, IJSonObject responseObject)
            : base(message)
        {
            Request = request;
            Response = response;
            JSonResponse = responseObject;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

#if !PocketPC && !WINDOWS_PHONE && !SILVERLIGHT && !WINDOWS_STORE
        /// <summary>
        /// Constructor required by serialization.
        /// </summary>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        private BayeuxException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            // PH: NOTE: as for now, I don't see a reason, why to also serialize other properties of this class beside message...
        }
#endif

        #region Properties

        /// <summary>
        /// Gets the request.
        /// </summary>
        public BayeuxRequest Request
        { get; private set; }

        /// <summary>
        /// Gets the response.
        /// </summary>
        public BayeuxResponse Response
        { get; private set; }

        /// <summary>
        /// Gets the JSON response received from server.
        /// </summary>
        public IJSonObject JSonResponse
        { get; private set; }

        #endregion
    }
}
