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
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using CodeTitans.Core.Dispatch;
using CodeTitans.Diagnostics;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
#if !WINDOWS_STORE
    /// <summary>
    /// Stub version of data-source. It is able to replay recorded responses for all incomming requests.
    /// It exposes additional SelectResponse notification, allowing customization of the way, which response is
    /// given for which request. By default one-by-one response is returned.
    /// 
    /// The idea behind is to use it instead of HttpDataSource for testing scenarios with preloaded and cached data.
    /// This data could be previously gathered (and maybe stored somewhere) if a dedicated debug-log listener is registered inside DebugLog class.
    /// </summary>
    public class RecordedDataSource : IHttpDataSource
    {
        /// <summary>
        /// Text printed in logs, when binary content is requested or received via this data-source.
        /// </summary>
        private const string BinaryContentDescription = "(binary content)";
        /// <summary>
        /// Text printed in logs, when nothing was sent or received.
        /// </summary>
        private const string NothingContentDescription = "(nothing)";

        private readonly IEventDispatcher _eventDispatcher;
        private readonly string _url;
        private bool _isActive;
        private readonly List<RecordedDataSourceResponse> _responses;
        private int _requestNumber;

        #region Implementation of IHttpDataSource

        /// <summary>
        /// Event fired each time new data has been received based on sent request.
        /// </summary>
        public event EventHandler<HttpDataSourceEventArgs> DataReceived;

        /// <summary>
        /// Event fired each time data reception failed due to any issue.
        /// </summary>
        public event EventHandler<HttpDataSourceEventArgs> DataReceiveFailed;

        /// <summary>
        /// Event fired each time response must be sent back to the caller of given request.
        /// It allows replace the standard algorithm, where the "next" item from internal collection
        /// of recorded responses is used.
        /// </summary>
        public event EventHandler<RecordedDataSourceSelectorEventArgs> SelectResponse;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedDataSource(IEventDispatcher eventDispatcher, string url, string contentType)
        {
            if (eventDispatcher == null)
                throw new ArgumentNullException("eventDispatcher");
            if (url == null)
                throw new ArgumentNullException("url");

            _eventDispatcher = eventDispatcher;
            _url = url;
            _responses = new List<RecordedDataSourceResponse>();
            Timeout = HttpDataSource.DefaultTimeout;
            ContentType = contentType;
        }

        /// <summary>
        /// Adds or replaces additional header sent via this data source with next request.
        /// </summary>
        public void AddHeader(string name, string value)
        {
        }

        /// <summary>
        /// Removes header with specified name.
        /// </summary>
        public void RemoveHeader(string name)
        {
        }

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        public void SendRequest(string method, HttpDataSourceResponseType responseType)
        {
            InternalSendRequest(new byte[0], 0, BinaryContentDescription, method, responseType);
        }

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        public void SendRequest(string relativeUrlPath, byte[] data, string method, HttpDataSourceResponseType responseType)
        {
            InternalSendRequest(data ?? new byte[0], data != null ? data.Length : 0, BinaryContentDescription,  method, responseType);
        }

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        public void SendRequest(string relativeUrlPath, byte[] data, int dataLength, string method, HttpDataSourceResponseType responseType)
        {
            var copy = data != null ? new byte[dataLength] : new byte[0];

            if (data != null)
                Array.Copy(data, copy, copy.Length);

            InternalSendRequest(copy, copy.Length, BinaryContentDescription, method, responseType);
        }

        /// <summary>
        /// Sends request to the data source.
        /// </summary>
        public void SendRequest(string relativeUrlPath, string data, string method, HttpDataSourceResponseType responseType)
        {
            var asBinary = data != null ? Encoding.UTF8.GetBytes(data) : new byte[0];
            InternalSendRequest(asBinary, data != null ? data.Length : 0, data, method, responseType);
        }

        private void InternalSendRequest(byte[] data, int dataLength, string dataDescription, string method, HttpDataSourceResponseType responseType)
        {
            VerifyRequest(data, dataLength, dataDescription, method);

            _isActive = true;
            ReceiveResponse(new RecordedDataSourceSelectorEventArgs(this, _responses, SelectedIndex, _requestNumber++, data, dataDescription, method, ContentType, Tag, responseType));
        }

        private void VerifyRequest(byte[] data, int dataLength, string dataDescription, string method)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            DebugLog.WriteCoreLine(string.Format(CultureInfo.InvariantCulture, "<--- Sending request (length: {0} bytes, at: {1}): {2} ({3}, {4})", dataLength, DateTime.Now, _url, method, ContentType));
            string category = string.Concat(DebugLog.CategoryCore, ".HttpDataSource.Send", string.IsNullOrEmpty(ContentType) ? string.Empty : ".", ContentType);
            DebugLog.WriteLine(category, dataLength > 0 ? dataDescription : NothingContentDescription);
        }

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        public void SendRequestAsync(string method, HttpDataSourceResponseType responseType)
        {
            InternalSendRequestAsync(new byte[0], 0, BinaryContentDescription, method, responseType);
        }

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        public void SendRequestAsync(string relativeUrlPath, byte[] data, string method, HttpDataSourceResponseType responseType)
        {
            InternalSendRequestAsync(data ?? new byte[0], data != null ? data.Length : 0, BinaryContentDescription, method, responseType);
        }

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        public void SendRequestAsync(string relativeUrlPath, byte[] data, int dataLength, string method, HttpDataSourceResponseType responseType)
        {
            var copy = data != null ? new byte[dataLength] : new byte[0];

            if (data != null)
                Array.Copy(data, copy, copy.Length);

            InternalSendRequestAsync(copy, copy.Length, BinaryContentDescription, method, responseType);
        }

        /// <summary>
        /// Sends asynchronous request to the data source.
        /// </summary>
        public void SendRequestAsync(string relativeUrlPath, string data, string method, HttpDataSourceResponseType responseType)
        {
            var asBinary = data != null ? Encoding.UTF8.GetBytes(data) : new byte[0];
            InternalSendRequestAsync(asBinary, data != null ? data.Length : 0, data, method, responseType);
        }

        private void InternalSendRequestAsync(byte[] data, int dataLength, string dataDescription, string method, HttpDataSourceResponseType responseType)
        {
            VerifyRequest(data, dataLength, dataDescription, method);

            _isActive = true;

            ThreadPool.QueueUserWorkItem(ResponseResponseAsync, new RecordedDataSourceSelectorEventArgs(this, _responses, SelectedIndex, _requestNumber++, data, dataDescription, method, ContentType, Tag, responseType));
        }

        private void ResponseResponseAsync(object state)
        {
            var args = (RecordedDataSourceSelectorEventArgs)state;
            ReceiveResponse(args);
        }

        private void ReceiveResponse(RecordedDataSourceSelectorEventArgs e)
        {
            if (e == null)
                throw new InvalidOperationException("Somehow lost required state argument!");

            DebugLog.WriteCoreLine("~~~> Waiting for stubbed response" + (string.IsNullOrEmpty(_url) ? string.Empty : string.Concat(" (", _url, ")")));

            InternalPreSelectResponse(e);

            // allow external code to select the next response,
            // if there is no handler, automatically next one from the list will be taken:
            Event.Invoke(SelectResponse, this, e);

            SelectedIndex = e.NextResponseIndex;
            var response = e.SelectedResponse;

            if (response == null)
            {
                DebugLog.WriteCoreLine("Missing response object!");

                _isActive = false;
                _eventDispatcher.Invoke(DataReceiveFailed, this, new HttpDataSourceEventArgs(this, HttpStatusCode.NotFound, "Not found response object, the request was adressed for"));
                return;
            }

            if (VerifyIfCancelled())
                return;

            // now wait time specified inside the response:
            if (response.Delay > 0)
            {
                if (response.Delay > Timeout)
                {
                    Thread.Sleep(Timeout);
                    if (VerifyIfCancelled())
                        return;

                    _isActive = false;
                    DebugLog.WriteCoreLine(string.Concat("--> Request timeouted (data-source timeout: ", Timeout, " ms, response delay: ", response.Delay, " ms)!"));
                    _eventDispatcher.Invoke(DataReceiveFailed, this, new HttpDataSourceEventArgs(this, HttpStatusCode.RequestTimeout, "Request timeouted due to huge response delay"));
                    return;
                }

                Thread.Sleep(response.Delay);
                if (VerifyIfCancelled())
                    return;

                _isActive = false;
            }

            InternalPreProcessResponse(e);

            DebugLog.WriteCoreLine(string.Format(CultureInfo.InvariantCulture, "---> Received response (length: {0} bytes, at: {1}, waiting: {2:F2} sec) with status: {3} ({4}, {5}, {6})",
                                                 response.Length, DateTime.Now, (DateTime.Now - e.SentAt).TotalSeconds, response.StatusCode, (int) response.StatusCode, response.StatusDescription, response.ContentType));

            string category = string.Concat(DebugLog.CategoryCore, ".HttpDataSource.Receive", string.IsNullOrEmpty(response.ContentType) ? string.Empty : ".", response.ContentType);

            if (response.Length == 0)
                DebugLog.WriteCoreLine(NothingContentDescription);
            else if (response.AsString != null)
                DebugLog.WriteLine(category, response.AsString);
            else
                DebugLog.WriteLine(category, BinaryContentDescription);

            switch (e.ResponseType)
            {
                case HttpDataSourceResponseType.AsString:
                    _eventDispatcher.Invoke(response.IsFailure ? DataReceiveFailed : DataReceived, this, new HttpDataSourceEventArgs(this, response.StatusCode, response.StatusDescription, response.AsString, null, null));
                    return;
                case HttpDataSourceResponseType.AsBinary:
                    _eventDispatcher.Invoke(response.IsFailure ? DataReceiveFailed : DataReceived, this, new HttpDataSourceEventArgs(this, response.StatusCode, response.StatusDescription, null, response.AsBinary, null));
                    return;
                case HttpDataSourceResponseType.AsRawStream:
                    using (var stream = new MemoryStream(response.AsBinary ?? new byte[0]))
                    {
                        _eventDispatcher.Invoke(response.IsFailure ? DataReceiveFailed : DataReceived, this, new HttpDataSourceEventArgs(this, response.StatusCode, response.StatusDescription, null, null, stream));
                    }
                    return;
                default:
                    throw new NotImplementedException("Invalid response type requested");
            }
        }

        private bool VerifyIfCancelled()
        {
            if (!_isActive)
            {
                DebugLog.WriteCoreLine("Ignoring request due to request cancellation recorded HttpDataSource response.");
                DebugLog.Break();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method called just before allowing externals to select a response.
        /// </summary>
        protected virtual void InternalPreSelectResponse(RecordedDataSourceSelectorEventArgs e)
        {
        }

        /// <summary>
        /// Method called just after selection of response performed, but before taking any other action.
        /// It might be used to:
        ///  1) update the data-source based on request data
        ///  2) update the response with request specific data
        ///  3) replace the whole response object
        ///  4) send some other extra notifications, when subclassed
        /// </summary>
        protected virtual void InternalPreProcessResponse(RecordedDataSourceSelectorEventArgs e)
        {
        }

        /// <summary>
        /// Cancels current operation.
        /// Throws an exception when nothing is actually processed.
        /// </summary>
        public void Cancel()
        {
            _isActive = false;
        }

        /// <summary>
        /// Gets an indication if there is an ongoing request.
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
        }

        /// <summary>
        /// Gets or sets the timeout for next requests (ms).
        /// </summary>
        public int Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the network login credentials.
        /// </summary>
        public NetworkCredential NetworkCredential
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user-agent name.
        /// </summary>
        public string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the HTTP condition of modified data.
        /// </summary>
        public DateTime IfModifiedSince
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any associated object with the request.
        /// </summary>
        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content type of message sent.
        /// </summary>
        public string ContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the content type accepted by data source.
        /// </summary>
        public string AcceptContentType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current index of response from the internal collection.
        /// </summary>
        public int SelectedIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of recorded responses.
        /// </summary>
        public IList<RecordedDataSourceResponse> Responses
        {
            get { return _responses; }
        }

        #endregion

        /// <summary>
        /// Records given response object.
        /// It will be later on used to serialize content for response on issued request.
        /// </summary>
        public RecordedDataSourceResponse Record(RecordedDataSourceResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            _responses.Add(response);
            return response;
        }

        /// <summary>
        /// Records additional string response.
        /// </summary>
        public RecordedDataSourceResponse Record(string response)
        {
            return Record(new RecordedDataSourceResponse("SUCCESS", response));
        }

        /// <summary>
        /// Records additional string response returned after a given delay.
        /// </summary>
        public RecordedDataSourceResponse Record(string response, int delay)
        {
            return Record(new RecordedDataSourceResponse("SUCCESS", response, delay));
        }

        /// <summary>
        /// Records additional binary response.
        /// </summary>
        public RecordedDataSourceResponse Record(byte[] response)
        {
            return Record(new RecordedDataSourceResponse("SUCCESS", response));
        }

        /// <summary>
        /// Records additional binary response returned after a given delay.
        /// </summary>
        public RecordedDataSourceResponse Record(byte[] response, int delay)
        {
            return Record(new RecordedDataSourceResponse("SUCCESS", response, delay));
        }

        /// <summary>
        /// Records additional failure response with given HTTP status-code.
        /// </summary>
        public RecordedDataSourceResponse RecordFailure(HttpStatusCode statusCode)
        {
            return Record(new RecordedDataSourceResponse("FAIL", null, null, RecordedDataSourceResponse.DefaultDelay, statusCode, statusCode.ToString()));
        }
    }
#endif
}
