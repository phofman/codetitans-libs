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
using System.Net;
using CodeTitans.JSon;
using CodeTitans.Bayeux.Responses;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Argument class passed together with <see cref="BayeuxConnection"/> events.
    /// </summary>
    public class BayeuxConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnectionEventArgs(BayeuxConnection connection, HttpStatusCode statusCode, string statusDescription)
        {
            Connection = connection;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnectionEventArgs(BayeuxConnection connection, HttpStatusCode statusCode, string statusDescription, string data, IJSonObject message)
            : this (connection, statusCode, statusDescription, data, message, null)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnectionEventArgs(BayeuxConnection connection, HttpStatusCode statusCode, string statusDescription, string data, IJSonObject message, BayeuxResponse response)
        {
            Connection = connection;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            Data = data;
            Message = message;
            Response = response;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnectionEventArgs(BayeuxConnection connection, HttpStatusCode statusCode, string statusDescription, string data, JSonReaderException exception)
        {
            Connection = connection;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            Data = data;
            Exception = exception;
        }

        #region Properties

        /// <summary>
        /// Gets the reference to the <see cref="BayeuxConnection"/> instance associated with this arguments.
        /// </summary>
        public BayeuxConnection Connection
        { get; private set; }

        /// <summary>
        /// Gets the current operation status code.
        /// </summary>
        public HttpStatusCode StatusCode
        { get; private set; }

        /// <summary>
        /// Gets the current operation description.
        /// </summary>
        public string StatusDescription
        { get; private set; }

        /// <summary>
        /// Gets the data received as the last message in case it can't be parsed as JSON structure.
        /// </summary>
        public string Data
        { get; private set; }

        /// <summary>
        /// Gets the JSON structured message received by <see cref="BayeuxConnection"/> associated with this arguments.
        /// </summary>
        public IJSonObject Message
        { get; private set; }

        /// <summary>
        /// Gets the exception causing data not to be correctly parsed.
        /// </summary>
        public JSonReaderException Exception
        { get; private set; }

        /// <summary>
        /// Gets the full response from the server.
        /// </summary>
        public BayeuxResponse Response
        { get; private set; }

        #endregion
    }
}
