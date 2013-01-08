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

using System.Net;
using System.Text;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Class that describes response that should be received on request made via IHttpDataSource.
    /// </summary>
    public class RecordedDataSourceResponse
    {
        public const int DefaultDelay = 50;
        public const HttpStatusCode DefaultStatusCode = HttpStatusCode.OK;

        private bool _isFailure;
        private byte[] _binaryData;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RecordedDataSourceResponse()
            : this(null)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSourceResponse(string name)
            : this(name, null, null, DefaultDelay, DefaultStatusCode, DefaultStatusCode.ToString())
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSourceResponse(string name, string responseAsString)
            : this(name, responseAsString, null, DefaultDelay, DefaultStatusCode, DefaultStatusCode.ToString())
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSourceResponse(string name, string responseAsString, int delay)
            : this(name, responseAsString, null, delay, DefaultStatusCode, DefaultStatusCode.ToString())
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSourceResponse(string name, byte[] responseAsBinary)
            : this(name, null, responseAsBinary, DefaultDelay, DefaultStatusCode, DefaultStatusCode.ToString())
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSourceResponse(string name, byte[] responseAsBinary, int delay)
            : this(name, null, responseAsBinary, delay, DefaultStatusCode, DefaultStatusCode.ToString())
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSourceResponse(string name, string responseAsString, byte[] responseAsBinary, int delay, HttpStatusCode statusCode, string statusDescription)
        {
            Name = name;
            Delay = delay;
            StatusCode = statusCode;
            StatusDescription = statusDescription;
            AsBinary = responseAsBinary;
            AsString = responseAsString;
        }

        /// <summary>
        /// Gets the name of this response.
        /// In case of 
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the delay, after which the response is
        /// </summary>
        public int Delay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response status code.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response status description.
        /// </summary>
        public string StatusDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the response content as binary.
        /// If no binary data was provided, it will return AsString value serialized as UTF8 binary array.
        /// </summary>
        public byte[] AsBinary
        {
            get
            {
                if (_binaryData != null)
                    return _binaryData;

                return Encoding.UTF8.GetBytes(AsString);
            }
            set { _binaryData = value; }
        }

        /// <summary>
        /// Gets or sets the response content as string.
        /// </summary>
        public virtual string AsString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the length of the response content.
        /// </summary>
        public int Length
        {
            get
            {
                if (AsString != null)
                    return AsString.Length;
                return _binaryData != null ? _binaryData.Length : 0;
            }
        }

        /// <summary>
        /// Gets or sets the indication, that given response is a failure one.
        /// </summary>
        public virtual bool IsFailure
        {
            get
            {
                if (_isFailure)
                    return true;

                return !(StatusCode >= HttpStatusCode.OK && StatusCode <= HttpStatusCode.Accepted);
            }
            set { _isFailure = value; }
        }

        /// <summary>
        /// Gets or sets any object associated with the request.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content-type of the response.
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }
    }
}
