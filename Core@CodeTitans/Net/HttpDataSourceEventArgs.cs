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
using System.IO;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Event data transmitted together with <see cref="HttpDataSource"/>.
    /// </summary>
    public class HttpDataSourceEventArgs : EventArgs
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public HttpDataSourceEventArgs(IHttpDataSource dataSource, HttpStatusCode statusCode, string statusDescription)
        {
            DataSource = dataSource;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public HttpDataSourceEventArgs(IHttpDataSource dataSource, HttpStatusCode statusCode, string statusDescription, string stringData, byte[] binaryData, Stream streamData)
        {
            DataSource = dataSource;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            StringData = stringData;
            BinaryData = binaryData;
            StreamData = streamData;
        }

        #region Properties

        /// <summary>
        /// Gets the reference to data source object associated with this args.
        /// </summary>
        public IHttpDataSource DataSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the data status.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the text description of the status.
        /// </summary>
        public string StatusDescription
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the received string data.
        /// </summary>
        public string StringData
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the received binary data.
        /// </summary>
        public byte[] BinaryData
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the received data.
        /// </summary>
        public Stream StreamData
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Gets the text representation of this object.
        /// </summary>
        public override string ToString()
        {
            if (StatusDescription != null)
                return StatusDescription;

            if (StringData != null)
                return StringData;
            if (BinaryData != null)
                return "(binary data)";
            if (StreamData != null)
                return "(stream data)";

            return "(no data)";
        }
    }
}
