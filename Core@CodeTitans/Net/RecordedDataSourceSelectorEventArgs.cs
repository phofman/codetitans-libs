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
using System.Collections.Generic;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
#if !WINDOWS_STORE
    /// <summary>
    /// Arguments passed along with SelectResponse event of RecordedDataSource class.
    /// </summary>
    public sealed class RecordedDataSourceSelectorEventArgs : EventArgs
    {
        private int _selectedResponseIndex;
        private RecordedDataSourceResponse _response;

        /// <summary>
        /// Hidden constructor.
        /// </summary>
        internal RecordedDataSourceSelectorEventArgs(RecordedDataSource dataSource, IList<RecordedDataSourceResponse> responses,
                                int selectedResponseIndex, int requestNumber, byte[] requestDataAsBinary, string requestDataAsString,
                                string requestMethod, string requestContentType, object requestTag, HttpDataSourceResponseType responseType)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");
            if (responses == null)
                throw new ArgumentNullException("responses");
            if (requestDataAsBinary == null)
                throw new ArgumentNullException("requestDataAsBinary");
            if (requestDataAsString == null)
                throw new ArgumentNullException("requestDataAsString");
            if (string.IsNullOrEmpty(requestMethod))
                throw new ArgumentNullException("requestMethod");

            DataSource = dataSource;
            Responses = responses;
            SelectedResponseIndex = selectedResponseIndex;
            RequestNumber = requestNumber;
            RequestDataAsBinary = requestDataAsBinary;
            RequestDataAsString = requestDataAsString;
            RequestMethod = requestMethod;
            RequestContentType = requestContentType;
            RequestTag = requestTag;
            ResponseType = responseType;
            SentAt = DateTime.Now;
        }

        #region Properties

        /// <summary>
        /// Gets the original data-source, which issues an event.
        /// </summary>
        public RecordedDataSource DataSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of already recorded responses and stored inside DataSource.
        /// </summary>
        public IList<RecordedDataSourceResponse> Responses
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the index of currently used response.
        /// </summary>
        public int SelectedResponseIndex
        {
            get { return _selectedResponseIndex; }
            set
            {
                _response = null;
                _selectedResponseIndex = value;
                NextResponseIndex = _selectedResponseIndex + 1;
            }
        }

        /// <summary>
        /// Gets or sets the index of response used for next request call.
        /// By default value is (SelectedResponseIndex + 1), so each requests consumes one response.
        /// </summary>
        public int NextResponseIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the selected response.
        /// This value is in sync with SelectedResponseIndex, but can also be overwritten, by custom object,
        /// created at runtime, and not comming from Responses collection. In this case SelectedResponseIndex gets the value of '-1'.
        /// </summary>
        public RecordedDataSourceResponse SelectedResponse
        {
            get
            {
                if (_response != null)
                    return _response;

                return _selectedResponseIndex >= 0 && _selectedResponseIndex < Responses.Count ? Responses[_selectedResponseIndex] : null;
            }
            set
            {
                _response = value;
                _selectedResponseIndex = value == null ? -1 : Responses.IndexOf(value);
                NextResponseIndex = _selectedResponseIndex + 1;
            }
        }

        /// <summary>
        /// Gets the sequential number, which stands for number of request-response calls performed already.
        /// </summary>
        public int RequestNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the binary data of the request.
        /// </summary>
        public byte[] RequestDataAsBinary
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the string data of the request.
        /// </summary>
        public string RequestDataAsString
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or set the JSON data associated with the request.
        /// </summary>
        public object RequestDataAsJSon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the request method used.
        /// </summary>
        public string RequestMethod
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the content-type of the request.
        /// </summary>
        public string RequestContentType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tag object associated with the request.
        /// </summary>
        public object RequestTag
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the expected response type.
        /// </summary>
        public HttpDataSourceResponseType ResponseType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the time, request was issued.
        /// </summary>
        public DateTime SentAt
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets any object associated with selection of response.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        #endregion
    }
#endif
}
