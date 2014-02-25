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
using System.Net;
using System.Text;
using CodeTitans.Bayeux.Requests;
using CodeTitans.Bayeux.Responses;
using CodeTitans.Diagnostics;
using CodeTitans.JSon;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Class for managing client Bayeux connection.
    /// </summary>
    public class BayeuxConnection : IDisposable
    {
        private const string DefaultContentType = "application/json";
        private const int DefaultRetryDelay = 5000;
        private const int DefaultNumberOfConnectRetries = 10;

        private readonly object _syncObject;
        private readonly IHttpDataSource _httpConnection;
        private readonly IHttpDataSource _httpLongPollingConnection;
        private string _clientID;
        private BayeuxConnectionState _state;
        private bool _longPolling;
        private BayeuxRequest _request;
        private BayeuxRequest _longPollingRequest;
        private int _longPollingConnecFailures;
        private readonly JSonReader _jsonReader;
        private readonly StringBuilder _writerCache;
        private readonly JSonWriter _jsonWriter;
        private readonly List<string> _subscribedChannels;

        #region Events

        /// <summary>
        /// Event fired when handshaking with remote server completed with success.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> Connected;
        /// <summary>
        /// Event fired when handshaking with remote server failed.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> ConnectionFailed;
        /// <summary>
        /// Event fired when disconnected from remote server (with special request or due to some critical errors).
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> Disconnected;
        /// <summary>
        /// Event fired each time data has been received from remote server.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> DataReceived;
        /// <summary>
        /// Event fired each time data has been received from remote server, but JSON parser failed to read it.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> DataFailed;
        /// <summary>
        /// Event fired when asynchronous event has been received.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> EventReceived;
        /// <summary>
        /// Event fired each time a message has been processed.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> ResponseReceived;
        /// <summary>
        /// Event fired, when long-polling connection failed to connect a given by LongPollingConnectRetries number of tries.
        /// </summary>
        public event EventHandler<BayeuxConnectionEventArgs> LongPollingFailed;

        #endregion

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnection(string url)
            : this(new HttpDataSource(url, null, DefaultContentType), new HttpDataSource(url, null, DefaultContentType))
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnection(IHttpDataSource connection)
            : this(connection, null)
        {
        }

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxConnection(IHttpDataSource connection, IHttpDataSource longPollingConnection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            _syncObject = new object();
            _writerCache = new StringBuilder();
            _jsonWriter = new JSonWriter(_writerCache, false);
            _jsonWriter.CompactEnumerables = true;
            _jsonReader = new JSonReader();
            _state = BayeuxConnectionState.Disconnected;
            _subscribedChannels = new List<string>();
            LongPollingRetryDelay = DefaultRetryDelay;
            LongPollingConnectRetries = DefaultNumberOfConnectRetries;

            _httpConnection = connection;
            _httpConnection.DataReceived += DataSource_OnDataReceived;
            _httpConnection.DataReceiveFailed += DataSource_OnDataReceiveFailed;

            if (longPollingConnection != null)
            {
                _httpLongPollingConnection = longPollingConnection;
                _httpLongPollingConnection.DataReceived += LongPollingDataSource_OnDataReceived;
                _httpLongPollingConnection.DataReceiveFailed += LongPollingDataSource_OnDataReceiveFailed;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~BayeuxConnection ()
        {
            Dispose(false);
        }

        #region Event Processing

        /// <summary>
        /// Helper method to fire Connected event.
        /// </summary>
        protected virtual void OnConnected(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(Connected, this, e);
        }

        /// <summary>
        /// Helper method to fire ConnectionFailed event.
        /// </summary>
        protected virtual void OnConnectionFailed(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(ConnectionFailed, this, e);
        }

        /// <summary>
        /// Helper method to fire Disconnected event.
        /// </summary>
        protected virtual void OnDisconnected(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(Disconnected, this, e);
        }

        /// <summary>
        /// Helper method to fire DataReceived event.
        /// </summary>
        protected virtual void OnDataReceived(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(DataReceived, this, e);
        }

        /// <summary>
        /// Helper method to fire DataFailed event.
        /// </summary>
        protected virtual void OnDataFailed(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(DataFailed, this, e);
        }

        /// <summary>
        /// Helper method to fire EventReceived event.
        /// </summary>
        protected virtual void OnEventReceived(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(EventReceived, this, e);
        }

        /// <summary>
        /// Helper method to fire ResponseReceived event.
        /// </summary>
        protected virtual void OnResponseReceived(BayeuxConnectionEventArgs e)
        {
            Event.Invoke(ResponseReceived, this, e);
        }

        #endregion

        private void DataSource_OnDataReceived(object sender, HttpDataSourceEventArgs e)
        {
            // say, there the current request is no more needed:
            BayeuxRequest r = _request;
            _request = null;
            ProcessReceivedData(r, e);
        }

        private void ProcessReceivedData(BayeuxRequest request, HttpDataSourceEventArgs e)
        {
            IJSonObject jsonData = null;

            try
            {
                // read JSON data:
                jsonData = _jsonReader.ReadAsJSonObject(e.StringData);
                OnDataReceived(new BayeuxConnectionEventArgs(this, e.StatusCode, e.StatusDescription, e.StringData, jsonData, null));
            }
            catch (JSonReaderException ex)
            {
                OnDataFailed(new BayeuxConnectionEventArgs(this, e.StatusCode, e.StatusDescription, e.StringData, ex));
            }

            if (jsonData != null)
            {
                // process JSON response, in case it is an array of responses:
                if (jsonData.IsArray)
                {
                    foreach (IJSonObject message in jsonData.ArrayItems)
                    {
                        ProcessResponseMessage(request, e.StatusCode, e.StatusDescription, message, e.StringData);
                    }
                }
                else
                {
                    ProcessResponseMessage(request, e.StatusCode, e.StatusDescription, jsonData, e.StringData);
                }
            }
            else
            {
                DebugLog.WriteBayeuxLine("--- No JSON data received from server ---");
                OnDataFailed(new BayeuxConnectionEventArgs(this, HttpStatusCode.NoContent, "No JSON data received"));
            }
        }

        private void DataSource_OnDataReceiveFailed(object sender, HttpDataSourceEventArgs e)
        {
            // say, there the current request is no more needed:
            BayeuxRequest r = _request;
            _request = null;

            if (r != null)
                r.ProcessFailed(e);

            OnDataFailed(new BayeuxConnectionEventArgs(this, e.StatusCode, e.StatusDescription));
        }

        private void LongPollingDataSource_OnDataReceived(object sender, HttpDataSourceEventArgs e)
        {
            ProcessReceivedData(_longPollingRequest, e);

            // reconnect if still connected:
            ReconnectLongPollingConnection(true);
        }

        private void LongPollingDataSource_OnDataReceiveFailed(object sender, HttpDataSourceEventArgs e)
        {
            if (_longPollingConnecFailures >= LongPollingConnectRetries)
            {
                Event.Invoke(LongPollingFailed, this, new BayeuxConnectionEventArgs(this, e.StatusCode, e.StatusDescription));
            }
            else
            {
                // ignore error and try to reconnect:
                Event.InvokeDelayed(LongPollingRetryDelay, ReconnectLongPollingConnectionAfterFailure, this, e);
            }
        }

        private void ReconnectLongPollingConnectionAfterFailure(object sender, EventArgs e)
        {
            ReconnectLongPollingConnection(false);
        }

        /// <summary>
        /// Sends asynchronously long-polling request again.
        /// </summary>
        protected void ReconnectLongPollingConnection(bool resetFailures)
        {
            if (resetFailures)
                _longPollingConnecFailures = 0;
            else
                _longPollingConnecFailures++;

            // try to reconnect, if possible:
            if (IsLongPolling && State == BayeuxConnectionState.Connected)
            {
                BayeuxRequest message = LongPollingRequest;
                string dataToSend;
                string requestMethod;

                lock (_syncObject)
                {
                    requestMethod = message.RequestMethod;
                    dataToSend = SerializeRequest(message);
                }

                _httpLongPollingConnection.SendRequestAsync(null, dataToSend, requestMethod, HttpDataSourceResponseType.AsString);
            }
        }

        /// <summary>
        /// Gets the current request that is being processed.
        /// </summary>
        public BayeuxRequest Request
        {
            get { return _request; }
        }

        /// <summary>
        /// Gets the request that will be sent to the remove server to poll the asynchronous data.
        /// </summary>
        public BayeuxRequest LongPollingRequest
        {
            get { return _longPollingRequest; }
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientID
        {
            get { return _clientID; }
            set
            {
                _clientID = value;
                _state = string.IsNullOrEmpty(_clientID) ? BayeuxConnectionState.Disconnected : BayeuxConnectionState.Connected;
            }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        public BayeuxConnectionState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets or sets indication if sent requests should be indented.
        /// </summary>
        public bool IndentedRequest
        {
            get { return _jsonWriter.Indent; }
            set { _jsonWriter.Indent = value; }
        }

        /// <summary>
        /// Gets or sets the timeout for next requests (ms).
        /// </summary>
        public int Timeout
        {
            get { return _httpConnection.Timeout; }
            set { _httpConnection.Timeout = value; }
        }

        /// <summary>
        /// Gets or sets the timeout of the long polling connection.
        /// </summary>
        public int LongPollingTimeout
        {
            get { return _httpLongPollingConnection != null ? _httpLongPollingConnection.Timeout : 0; }
            set
            {
                if (_httpLongPollingConnection != null)
                    _httpLongPollingConnection.Timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of tries, the long-polling connection uses to reestabilish
        /// a connection with server in case of network failures.
        /// </summary>
        public int LongPollingConnectRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the delay between long polling connection retries.
        /// </summary>
        public int LongPollingRetryDelay
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the network login credentials.
        /// </summary>
        public NetworkCredential NetworkCredential
        {
            get { return _httpConnection.NetworkCredential; }
            set
            {
                _httpConnection.NetworkCredential = value;
                if (_httpLongPollingConnection != null)
                    _httpLongPollingConnection.NetworkCredential = value;
            }
        }

        /// <summary>
        /// Gets or sets the user-agent name.
        /// </summary>
        public string UserAgent
        {
            get { return _httpConnection.UserAgent; }
            set
            {
                _httpConnection.UserAgent = value;
                if (_httpLongPollingConnection != null)
                    _httpLongPollingConnection.UserAgent = value;
            }
        }

        /// <summary>
        /// Gets indication if secondary connection is polling server for asynchronous messages.
        /// </summary>
        public bool IsLongPolling
        {
            get { return _longPolling && _httpLongPollingConnection != null && _longPollingRequest != null; }
        }

        /// <summary>
        /// Adds or replaces additional header sent via this connection with next request.
        /// </summary>
        public void AddHeader(string name, string value)
        {
            _httpConnection.AddHeader(name, value);
            if (_httpLongPollingConnection != null)
                _httpLongPollingConnection.AddHeader(name, value);
        }

        /// <summary>
        /// Removes header with specified name.
        /// </summary>
        public void RemoveHeader(string name)
        {
            _httpConnection.RemoveHeader(name);
            if (_httpLongPollingConnection != null)
                _httpLongPollingConnection.RemoveHeader(name);
        }

        #region Bayeux Handshake

        /// <summary>
        /// Sends handshake request to the server.
        /// </summary>
        public void Handshake(BayeuxConnectionTypes supportedConnectionTypes, IJSonWritable data, IJSonWritable ext, bool asynchronous)
        {
            // if there is another Handshake request executed in background, try to cancel it first:
            if (_state == BayeuxConnectionState.Connecting)
                Cancel(); // <-- this should reset the state to Disconnected!

            if (_state != BayeuxConnectionState.Disconnected)
                throw new InvalidOperationException("Connecting or already connected to bayeux server! Disconnect first");

            _state = BayeuxConnectionState.Connecting;
            SendRequest(new HandshakeRequest(supportedConnectionTypes, data, ext), asynchronous);
        }

        /// <summary>
        /// Sends handshake request to the server.
        /// </summary>
        public void Handshake(BayeuxConnectionTypes supportedConnectionTypes, IJSonWritable data, string userName, string password, bool asynchronous)
        {
            Handshake(supportedConnectionTypes, data, new BayeuxHandshakeExtension(userName, password), asynchronous);
        }

        /// <summary>
        /// Sends handshake request to the server.
        /// </summary>
        public void Handshake()
        {
            Handshake(DefaultHandshakeConnectionTypes, DefaultHandshakeData, DefaultHandshakeExt, true);
        }

        /// <summary>
        /// Sends handshake request to the server in a synchronous way.
        /// Useful only when testing as blocks the current thread.
        /// </summary>
        public void HandshakeSync()
        {
            Handshake(DefaultHandshakeConnectionTypes, DefaultHandshakeData, DefaultHandshakeExt, false);
        }

        #region Handshake Virtual Methods

        /// <summary>
        /// Gets the default connection types presented while handshaking.
        /// </summary>
        protected virtual BayeuxConnectionTypes DefaultHandshakeConnectionTypes
        {
            get
            {
                return BayeuxConnectionTypes.LongPolling | BayeuxConnectionTypes.CallbackPolling | BayeuxConnectionTypes.Iframe;
            }
        }

        /// <summary>
        /// Gets the default data presented while handshaking.
        /// </summary>
        protected virtual IJSonWritable DefaultHandshakeData
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the default ext presented while handshaking.
        /// </summary>
        protected virtual IJSonWritable DefaultHandshakeExt
        {
            get { return null; }
        }

        #endregion

        #endregion

        #region Bayeux Connect

        /// <summary>
        /// Sends connect request to the server.
        /// </summary>
        public void Connect(IJSonWritable data, IJSonWritable ext, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the server - handshake must be performed first");

            SendRequest(new ConnectRequest(ClientID, BayeuxConnectionTypes.LongPolling, data, ext), asynchronous);
        }

        /// <summary>
        /// Sends connect request to the server.
        /// </summary>
        public void Connect()
        {
            Connect(null, null, true);
        }

        /// <summary>
        /// Sends connect request to the server in a synchronous way.
        /// Useful only when testing as blocks the current thread.
        /// </summary>
        public void ConnectSync()
        {
            Connect(null, null, false);
        }

        #endregion

        #region Bayeux Disconnect

        /// <summary>
        /// Sends disconnect request to the server.
        /// </summary>
        public void Disconnect(IJSonWritable data, IJSonWritable ext, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the server - handshake must be performed first");

            SendRequest(new DisconnectRequest(ClientID, data, ext), asynchronous);
        }

        /// <summary>
        /// Sends disconnect request to the server.
        /// </summary>
        public void Disconnect()
        {
            Disconnect(null, null, true);
        }

        /// <summary>
        /// Sends disconnect request to the server in a synchronous way.
        /// Useful only when testing as blocks the current thread.
        /// </summary>
        public void DisconnectSync()
        {
            Disconnect(null, null, false);
        }

        /// <summary>
        /// Changes instantly state to disconnected without any communication.
        /// </summary>
        public void AbandonConnection()
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected to server, nothing to abandon");

            ClientID = null;
        }

        #endregion

        #region Bayeux Subscribe

        /// <summary>
        /// Sends subscription request to the server.
        /// </summary>
        public void Subscribe(IJSonWritable data, IJSonWritable ext, string channel, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the server - handshake must be performed first");

            if (Subscribed(channel))
                throw new BayeuxException(string.Format(CultureInfo.InvariantCulture, "Already subscribed to channel: '{0}'", channel));

            SendRequest(new SubscribeRequest(ClientID, channel, data, ext), asynchronous);
        }

        /// <summary>
        /// Sends subscription request to the server.
        /// </summary>
        public void Subscribe(string channel)
        {
            Subscribe(null, null, channel, true);
        }

        /// <summary>
        /// Sends subscription request to the server in a synchronous way.
        /// Useful only when testing as blocks the current thread.
        /// </summary>
        public void SubscribeSync(string channel)
        {
            Subscribe(null, null, channel, false);
        }

        #endregion

        #region Bayeux Unsubscribe

        /// <summary>
        /// Sends unsubscription request to the server.
        /// </summary>
        public void Unsubscribe(IJSonWritable data, IJSonWritable ext, string channel, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the server - handshake must be performed first");

            if (!Subscribed(channel))
                throw new BayeuxException(string.Format(CultureInfo.InvariantCulture, "Not subscribed to channel: '{0}'", channel));

            SendRequest(new UnsubscribeRequest(ClientID, channel, data, ext), asynchronous);
        }

        /// <summary>
        /// Sends unsubscription request to the server.
        /// </summary>
        public void Unsubscribe(string channel)
        {
            Unsubscribe(null, null, channel, true);
        }

        /// <summary>
        /// Sends unsubscription request to the server in a synchronous way.
        /// Useful only when testing as blocks the current thread.
        /// </summary>
        public void UnsubscribeSync(string channel)
        {
            Unsubscribe(null, null, channel, false);
        }

        #endregion

        #region Bayeux Publish

        /// <summary>
        /// Sends publish request to the server.
        /// </summary>
        public void Publish(IJSonWritable data, IJSonWritable ext, string channel, IJSonWritable eventData, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected to the server - handshake must be performed first");

            SendRequest(new PublishRequest(ClientID, channel, eventData, data, ext), asynchronous);
        }

        /// <summary>
        /// Sends subscription request to the server.
        /// </summary>
        public void Publish(string channel, IJSonWritable eventData)
        {
            Publish(null, null, channel, eventData, true);
        }

        /// <summary>
        /// Sends subscription request to the server in a synchronous way.
        /// Useful only when testing as blocks the current thread.
        /// </summary>
        public void PublishSync(string channel, IJSonWritable eventData)
        {
            Publish(null, null, channel, eventData, false);
        }

        #endregion

        #region Bayeux Long-Polling

        /// <summary>
        /// Starts another long-polling HTTP connection to asynchronously receive events from Bayeux server.
        /// </summary>
        public void StartLongPolling()
        {
            StartLongPolling(new ConnectRequest(ClientID, BayeuxConnectionTypes.LongPolling, null, null));
        }

        /// <summary>
        /// Starts another long-polling HTTP connection to asynchronously receive events from Bayeux server.
        /// </summary>
        public void StartLongPolling(BayeuxRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (_httpLongPollingConnection == null)
                throw new InvalidOperationException("Long-polling connection not specified during BayeuxConnection object creation");
            if (State != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected");
            if (IsLongPolling)
                throw new InvalidOperationException("Connection is already polling");

            _longPollingRequest = request;
            _longPolling = true;
            ReconnectLongPollingConnection(true);
        }

        /// <summary>
        /// Stops listening for asynchronous data.
        /// </summary>
        public void StopLongPolling()
        {
            if (IsLongPolling)
            {
                _longPollingRequest = null;
                _longPolling = false;
                _httpLongPollingConnection.Cancel();
            }
        }

        #endregion

        /// <summary>
        /// Sends a bayeux message to the server. The message must be prepared first as a JSON string.
        /// Connection must be established first otherwise the exception will be thrown.
        /// </summary>
        public void Send(string message, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected");

            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException("message");

            Send(null, message, asynchronous);
        }

        /// <summary>
        /// Sends a bayeux message to the server.
        /// Connection must be established first otherwise the exception will be thrown.
        /// </summary>
        public void Send(BayeuxRequest message, bool asynchronous)
        {
            if (_state != BayeuxConnectionState.Connected)
                throw new InvalidOperationException("Not connected");

            if (message == null)
                throw new ArgumentNullException("message");

            Send(message, null, asynchronous);
        }

        /// <summary>
        /// Sends a bayeux message to the server with the ability to override the JSON returned by request object itself
        /// with the one specified as raw JSON data.
        /// </summary>
        public void Send(BayeuxRequest message, string overrideWithJSonData, bool asynchronous)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            string dataToSend = overrideWithJSonData;

            lock (_syncObject)
            {
                if (_request != null)
                {
                    if (_httpConnection.IsActive)
                        _httpConnection.Cancel(); // this is a sync operation, which should cause the _request to be nulled ...
                    _request = null;
                }

                if (_request != null)
                    throw new InvalidOperationException("Can't start new request, when current has not been finished/cancelled");

                if (string.IsNullOrEmpty(message.ClientID))
                    message.ClientID = ClientID;

                _request = message;

                // serialize JSON data as text message if not provided as parameter:
                if (string.IsNullOrEmpty(overrideWithJSonData))
                    dataToSend = SerializeRequest(message);
            }

            if (asynchronous)
                _httpConnection.SendRequestAsync(null, dataToSend, message.RequestMethod, HttpDataSourceResponseType.AsString);
            else
                _httpConnection.SendRequest(null, dataToSend, message.RequestMethod, HttpDataSourceResponseType.AsString);
        }

        /// <summary>
        /// Converts <see cref="BayeuxRequest"/> to string containing its JSON representation.
        /// </summary>
        protected string SerializeRequest(BayeuxRequest request)
        {
            _writerCache.Remove(0, _writerCache.Length);
            _jsonWriter.WriteArrayBegin();
            request.Write(_jsonWriter);
            _jsonWriter.WriteArrayEnd();
            return _jsonWriter.ToString();
        }

        /// <summary>
        /// Sends bayeux message.
        /// </summary>
        protected void SendRequest(BayeuxRequest message, bool asynchronous)
        {
            Send(message, null, asynchronous);
        }

        /// <summary>
        /// Cancels the currently executed asynchronous server request.
        /// </summary>
        public void Cancel()
        {
            if (_httpConnection.IsActive)
                _httpConnection.Cancel();

            // check if cancelled the handshake request:
            if (_state == BayeuxConnectionState.Connecting)
                _state = BayeuxConnectionState.Disconnected;
        }

        /// <summary>
        /// Checks if subscribed to given channel.
        /// </summary>
        public bool Subscribed(string channel)
        {
            return _subscribedChannels.Contains(channel);
        }

        #region Responses

        /// <summary>
        /// Get the custom bayeux-response associated with given message.
        /// By default returns a generic response.
        /// </summary>
        protected virtual BayeuxResponse ProvideResponse(IJSonObject message)
        {
             return new BayeuxResponse(message);
        }

        #endregion

        private void ProcessResponseMessage(BayeuxRequest request, HttpStatusCode httpStatusCode, string httpStatusDescription, IJSonObject message, string rawMessage)
        {
            try
            {

                string channel = message["channel"].StringValue; // each Bayuex message must have a channel associated!
                BayeuxResponse response = null;

                if (string.IsNullOrEmpty(channel))
                    throw new BayeuxException("Unexpected message with empty channel!");
                if (request != null && (channel.StartsWith("/meta", StringComparison.Ordinal) && channel != request.Channel))
                    throw new BayeuxException(string.Format(CultureInfo.InvariantCulture, "Unexpected response with channel: '{0}'", channel), request, null, message);
                if (request != null && channel.StartsWith("/meta", StringComparison.Ordinal) && request.ID != message["id"].StringValue)
                    throw new BayeuxException(string.Format(CultureInfo.InvariantCulture, "Invalid response ID, current: '{0}', expected: '{1}'", request.ID, message["id"].StringValue), request, null,
                                              message);

                ///////////////////////////////////////////////////////////////////////////////////////////
                // identify meta messages:
                if (channel == HandshakeRequest.MetaChannel)
                {
                    var handshakeResponse = new HandshakeResponse(message);
                    response = handshakeResponse;

                    // inform, that connection succeeded:
                    if (handshakeResponse.Successful)
                    {
                        ClientID = response.ClientID;

                        if (string.IsNullOrEmpty(ClientID))
                        {
                            throw new BayeuxException("Invalid ClientID received from server", request, handshakeResponse, message);
                        }

                        OnConnected(new BayeuxConnectionEventArgs(this, httpStatusCode, httpStatusDescription, rawMessage, message, handshakeResponse));
                    }
                    else
                    {
                        // inform that Handshake failed, via dedicated event:
                        ClientID = null;
                        OnConnectionFailed(new BayeuxConnectionEventArgs(this, response.Successful ? HttpStatusCode.OK : HttpStatusCode.BadRequest, null, rawMessage, message, response));
                    }
                }

                if (channel == DisconnectRequest.MetaChannel)
                {
                    response = new BayeuxResponse(message);

                    // inform that disconnection succeeded:
                    _state = BayeuxConnectionState.Disconnected;
                    ClientID = null;

                    OnDisconnected(new BayeuxConnectionEventArgs(this, response.Successful ? HttpStatusCode.OK : HttpStatusCode.BadRequest, null, rawMessage, message, response));
                }

                if (channel == SubscribeRequest.MetaChannel)
                {
                    var subscribeResponse = new SubscribeResponse(message);
                    response = subscribeResponse;

                    if (subscribeResponse.Successful)
                        _subscribedChannels.Add(subscribeResponse.SubscriptionChannel);
                }

                if (channel == UnsubscribeRequest.MetaChannel)
                {
                    var unsubscribeResponse = new UnsubscribeResponse(message);
                    response = unsubscribeResponse;

                    if (unsubscribeResponse.Successful)
                        _subscribedChannels.Remove(unsubscribeResponse.SubscriptionChannel);
                }

                if (_subscribedChannels.Contains(channel))
                {
                    response = new BayeuxResponse(message);

                    // event from server:
                    OnEventReceived(new BayeuxConnectionEventArgs(this, httpStatusCode, httpStatusDescription, rawMessage, message, response));
                }

                // process generic response:
                if (response == null)
                    response = ProvideResponse(message);

                // allow filtering of ResponseReceived event:
                if (ProcessResponse(request, response))
                    return;

                // one place to process all responses:
                OnResponseReceived(new BayeuxConnectionEventArgs(this, httpStatusCode, httpStatusDescription, rawMessage, message, response));
            }
            catch (Exception ex)
            {
                DebugLog.WriteBayeuxException(ex);

                string statusDescription = string.Concat(httpStatusDescription, "; unexpected exception: \"", ex.Message, "\"");
                OnDataFailed(new BayeuxConnectionEventArgs(this, httpStatusCode, statusDescription, rawMessage, message));
            }
        }

        /// <summary>
        /// Process given response. Request is also provided.
        /// The default behavior is that it only passes response to request, however there might be
        /// a situation, when no request was given (when a 'text' message has been sent).
        /// Returning 'true' as a result will filter out the ResponseReceived event.
        /// </summary>
        protected virtual bool ProcessResponse(BayeuxRequest request, BayeuxResponse response)
        {
            if (request != null)
                request.ProcessResponse(response);
            else
            {
                DebugLog.WriteBayeuxLine("BayeuxConnection: Processing response ignored for null request!");
            }

            return false;
        }

        #region IDisposable Members

        /// <summary>
        /// Release all native resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release all native resources used by this object.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_jsonWriter != null)
                    _jsonWriter.Dispose();
            }
        }

        #endregion
    }
}
