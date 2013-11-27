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

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Interface defining communication protocol between HTTP client and its abstract data source.
    /// </summary>
    public interface IHttpDataSource
    {
        /// <summary>
        /// Event fired each time new data has been received based on sent request.
        /// </summary>
        event EventHandler<HttpDataSourceEventArgs> DataReceived;
        /// <summary>
        /// Event fired each time data reception failed due to any issue.
        /// </summary>
        event EventHandler<HttpDataSourceEventArgs> DataReceiveFailed;

        /// <summary>
        /// Adds or replaces additional header sent via this data source with next request.
        /// </summary>
        void AddHeader(string name, string value);

        /// <summary>
        /// Removes header with specified name.
        /// </summary>
        void RemoveHeader(string name);

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        void SendRequest(string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        void SendRequest(string relativeUrlPath, byte[] data, string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        void SendRequest(string relativeUrlPath, byte[] data, int dataLength, string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        void SendRequest(string relativeUrlPath, string data, string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        void SendRequestAsync(string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        void SendRequestAsync(string relativeUrlPath, byte[] data, string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        void SendRequestAsync(string relativeUrlPath, byte[] data, int dataLength, string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        void SendRequestAsync(string relativeUrlPath, string data, string method, HttpDataSourceResponseType responseType);

        /// <summary>
        /// Cancels current operation.
        /// Throws an exception when nothing is actually processed.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Gets an indication if there is an ongoing request.
        /// </summary>
        bool IsActive
        {
            get;
        }

        /// <summary>
        /// Gets or sets the timeout for next requests (ms).
        /// </summary>
        int Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the network login credentials.
        /// </summary>
        NetworkCredential NetworkCredential
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user-agent name.
        /// </summary>
        string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any associated object with the request.
        /// </summary>
        object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content type of message sent.
        /// </summary>
        string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the content type accepted by data source.
        /// </summary>
        string AcceptContentType
        {
            get;
            set;
        }
    }
}
