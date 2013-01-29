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
using System.IO;
using System.Reflection;
using System.Text;
using CodeTitans.Core.Dispatch;
using CodeTitans.Diagnostics;
using CodeTitans.JSon;

#if !CODETITANS_LIB_CORE
namespace CodeTitans.Bayeux
#else
namespace CodeTitans.Core.Net
#endif
{
    /// <summary>
    /// Stub version of IHttpDataSource. It allows playback of previously recorded set of responses.
    /// It extends the default RecordedDataSource with ability to track bayeux-specific fields
    /// (i.e.: request ID, Token and ClientID) and automatically update them based on current requests/responses traffic.
    /// </summary>
    public sealed class RecordedBayeuxDataSource : RecordedDataSource
    {
        private const string DefaultToken = "RecToken-1";

        /// <summary>
        /// Event fired each time just before returning response to caller.
        /// It is designed to allow some custom fields update, based on the request or any other data avaiable at the moment.
        /// </summary>
        public event EventHandler<RecordedBayeuxDataSourceUpdateEventArgs> UpdateResponse;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public RecordedBayeuxDataSource(IEventDispatcher eventDispatcher, string url, string contentType, bool allowUpdateToken)
            : base(eventDispatcher, url, contentType)
        {
            Token = DefaultToken;
            ClientID = "0xDEADBEEF";
            AllowUpdateTokenOnRequest = allowUpdateToken;
        }

        /// <summary>
        /// Method called just before allowing externals to select a response.
        /// </summary>
        protected override void InternalPreSelectResponse(RecordedDataSourceSelectorEventArgs e)
        {
            if (!AllowMatchingByChannel)
                return;

            var request = ConvertToJSon(e);

            // do nothing:
            if (request == null || !request.IsEnumerable)
                return;

            var channel = RecordedBayeuxDataSourceResponse.GetChannel(request);
            if (string.IsNullOrEmpty(channel))
                return;

            // select matching response by channel:
            foreach (var response in e.Responses)
            {
                var bayeuxResponse = response as RecordedBayeuxDataSourceResponse;
                if (bayeuxResponse != null)
                {
                    if (string.Compare(channel, bayeuxResponse.Channel, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        e.SelectedResponse = bayeuxResponse;

                        DebugLog.WriteBayeuxLine(string.Concat("Selected response for channel: '", channel, "'"));
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Method called just after selection of response performed, but before taking any other action.
        /// It might be used to:
        ///  1) update the data-source based on request data
        ///  2) update the response with request specific data
        ///  3) replace the whole response object
        ///  4) send some other extra notifications, when subclassed
        /// </summary>
        protected override void InternalPreProcessResponse(RecordedDataSourceSelectorEventArgs e)
        {
            // find fields inside the request:
            string id = ReadSharedFieldsAndID(ConvertToJSon(e));

            // update token value, if allowed:
            if (!AllowUpdateTokenOnRequest)
                UpdateTokenValue();

            // update response fields, depending if it is a single or array response:
            var response = e.SelectedResponse as RecordedBayeuxDataSourceResponse;
            if (response != null)
            {
                if (response.AsJSon != null)
                {
                    UpdateSharedFields(e.RequestDataAsJSon as IJSonObject, response.AsJSon, id);
                }
                else
                {
                    IJSonObject responseJSon = null;
                    try
                    {
                        var reader = new JSonReader();
                        responseJSon = reader.ReadAsJSonObject(response.AsString);
                    }
                    catch (Exception ex)
                    {
                        DebugLog.WriteBayeuxLine("Response AsString is not in JSON format!");
                        DebugLog.WriteBayeuxException(ex);
                    }

                    UpdateSharedFields(e.RequestDataAsJSon as IJSonObject, responseJSon, id);
                }
            }
        }

        /// <summary>
        /// Update token value. The idea behind is that each token ends with '-{number}' and on each call the number should be incremented.
        /// </summary>
        private void UpdateTokenValue()
        {
            if (string.IsNullOrEmpty(Token))
                Token = DefaultToken;
            else
            {
                var numberStartAt = Token.LastIndexOf('-');

                if (numberStartAt < 0)
                    Token += "-1";
                else
                {
                    var numberString = Token.Substring(numberStartAt + 1);
                    int number;
                    if (int.TryParse(numberString, out number))
                    {
                        number++;
                        Token = (numberStartAt > 0 ? Token.Substring(0, numberStartAt) : string.Empty) + "-" + number;
                    }
                    else
                    {
                        Token += "-1";
                    }
                }
            }
        }

        private IJSonObject ConvertToJSon(string data)
        {
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var reader = new JSonReader();
                    return reader.ReadAsJSonObject(data);
                }
                catch (Exception ex)
                {
                    DebugLog.WriteBayeuxLine("Request is not in JSON format!");
                    DebugLog.WriteBayeuxException(ex);
                }
            }

            return null;
        }

        private IJSonObject ConvertToJSon(RecordedDataSourceSelectorEventArgs e)
        {
            if (e == null)
                return null;

            var existingJSon = e.RequestDataAsJSon as IJSonObject;
            if (existingJSon != null)
                return existingJSon;

            var data = ConvertToJSon(e.RequestDataAsString);
            e.RequestDataAsJSon = data;
            return data;
        }

        private void UpdateSharedFields(IJSonObject request, IJSonObject response, string id)
        {
            if (response != null && response.IsEnumerable)
            {
                if (response.IsArray)
                {
                    foreach (var singleResponseMessage in response.ArrayItems)
                        UpdateSingleResponse(request, singleResponseMessage, id);
                }
                else
                {
                    UpdateSingleResponse(request, response, id);
                }
            }
        }

        private string ReadSharedFieldsAndID(IJSonObject request)
        {
            // if request in unsupported format:
            if (request == null || !request.IsEnumerable)
            {
                DebugLog.WriteBayeuxLine("Request is not in supported JSON format!");
                return null;
            }

            if (request.IsArray)
            {
                // return ID of first message:
                foreach (var r in request.ArrayItems)
                    return ReadSharedFieldsSingleRequest(r);

                return null;
            }

            return ReadSharedFieldsSingleRequest(request);
        }

        private string ReadSharedFieldsSingleRequest(IJSonObject r)
        {
            ClientID = r["clientId", ClientID].StringValue;
            if (r.Contains("ext"))
                Token = r["ext"]["token", Token].StringValue;
            return r["id", (string) null].StringValue;
        }

        private void UpdateSingleResponse(IJSonObject request, IJSonObject response, string id)
        {
            if (response == null)
                return;

            // update response 'id' and 'clientId' fields:
            if (!response.IsMutable)
                return;

            var mutableResponse = response.AsMutable;
            if (mutableResponse == null || !mutableResponse.IsEnumerable)
                return;

            if (id == null)
                mutableResponse.Remove("id");
            else
                mutableResponse.SetValue("id", id);

            mutableResponse.SetValue("clientId", ClientID);

            // update response 'token' field:
            var responseExtObject = mutableResponse["ext", (IJSonObject) null];

            if (responseExtObject != null && responseExtObject.IsMutable)
                responseExtObject.AsMutable.SetValue("token", Token);

            // send update notification for a request that is an array of bayeux messages,
            // or if allowed, for the first message from that array:
            if (!AllowUpdateNotificationWithFirstMessageItemOnly || request == null || !request.IsEnumerable || !request.IsArray)
            {
                Event.Invoke(UpdateResponse, this, new RecordedBayeuxDataSourceUpdateEventArgs(this, request, mutableResponse));
            }
            else
            {
                foreach (var item in request.ArrayItems)
                {
                    Event.Invoke(UpdateResponse, this, new RecordedBayeuxDataSourceUpdateEventArgs(this, item, mutableResponse));
                    return;
                }

                // in case it's empty array:
                Event.Invoke(UpdateResponse, this, new RecordedBayeuxDataSourceUpdateEventArgs(this, null, mutableResponse));
            }
        }

        #region Properties

        /// <summary>
        /// Gets the last request 'token' value (or default if first request is about to be performed).
        /// </summary>
        public string Token
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the last request 'clientId' value.
        /// </summary>
        public string ClientID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an indication, if token should be updated as a result of each request.
        /// </summary>
        public bool AllowUpdateTokenOnRequest
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an indication, if responses should be matched by bayeux-channel of the request.
        /// </summary>
        public bool AllowMatchingByChannel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an indication, if UpdateResponse notifications should be send only with first bayeux message of the request.
        /// </summary>
        public bool AllowUpdateNotificationWithFirstMessageItemOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an indication, if custom data inside the response could be updated, based on the incomming request.
        /// </summary>
        public bool AllowCustomDataUpdate
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Records bayeux-handshake response.
        /// </summary>
        public RecordedBayeuxDataSourceResponse RecordBayeuxHandshake(BayeuxConnectionTypes connectionTypes)
        {
            var responseJSonText =
                "{\"supportedConnectionTypes\":[\"" + string.Join(", ", BayeuxConnectionTypesHelper.ToCollection(connectionTypes)) +
                "\"],\"ext\":{\"token\":\"xxx\"},\"successful\":true,\"authSuccessful\":true,\"clientId\":\"xxx\",\"advice\":{\"reconnect\":\"retry\"},\"version\":\"1.0\",\"channel\":\"/meta/handshake\",\"minimumVersion\":\"1.0\"}";

            return RecordBayeux("Handshake", responseJSonText);
        }

        /// <summary>
        /// Records a response as bayeux message.
        /// </summary>
        public RecordedBayeuxDataSourceResponse RecordBayeux(string name, string messageAsJSon)
        {
            var reader = new JSonReader();
            var response = new RecordedBayeuxDataSourceResponse(name);

            response.AsJSon = reader.ReadAsJSonMutableObject(messageAsJSon);

            Record(response);
            return response;
        }

        /// <summary>
        /// Records a response as bayeux message.
        /// </summary>
        public RecordedBayeuxDataSourceResponse RecordBayeux(string name, IJSonObject messageAsJSon)
        {
            if (messageAsJSon == null)
                throw new ArgumentNullException("messageAsJSon");

            var response = new RecordedBayeuxDataSourceResponse(name);
            response.AsJSon = messageAsJSon;

            Record(response);
            return response;
        }

        /// <summary>
        /// Records a response as bayeux message, while only 'data' part of the message is given.
        /// </summary>
        public RecordedDataSourceResponse RecordBayeuxData(string name, int internalStatus, string channel, IJSonObject data)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException("channel");

            var response = new RecordedBayeuxDataSourceResponse(name);
            var bayeuxMessage = CreateEmptyBayeuxResponse(internalStatus, channel);

            bayeuxMessage.SetValue("data", data);
            response.AsJSon = bayeuxMessage;

            Record(response);
            return response;
        }

        /// <summary>
        /// Records a response as bayeux message, while only 'data' part of the message is given.
        /// It will try to parse data and put as IJSonMutableObject, or if parsing fails, it will put it
        /// just as string value of the field.
        /// </summary>
        public RecordedDataSourceResponse RecordBayeuxData(string name, int internalStatus, string channel, string data)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException("channel");

            var response = new RecordedBayeuxDataSourceResponse(name);
            var bayeuxMessage = CreateEmptyBayeuxResponse(internalStatus, channel);

            if (string.IsNullOrEmpty(data))
            {
                bayeuxMessage.SetValue("data", data);
            }
            else
            {
                try
                {
                    var reader = new JSonReader();
                    bayeuxMessage.SetValue("data", reader.ReadAsJSonMutableObject(data));
                }
                catch (Exception ex)
                {
                    DebugLog.WriteBayeuxLine("Parsing of 'data' for RecordedBayeuxDataSourceResponse failed!");
                    DebugLog.WriteBayeuxException(ex);

                    bayeuxMessage.SetValue("data", data);
                }
            }

            response.AsJSon = bayeuxMessage;

            Record(response);
            return response;
        }

        /// <summary>
        /// Creates new IJSonMutableObject containing all basic fields for typical bayeux response message.
        /// </summary>
        public IJSonMutableObject CreateEmptyBayeuxResponse(int internalStatus, string channel)
        {
            if (string.IsNullOrEmpty(channel))
                throw new ArgumentNullException("channel");

            var message = JSonMutableObject.CreateDictionary();

            message.SetDictionary("data");

            var ext = message.SetDictionary("ext");
            if (internalStatus >= 0)
                ext.SetValue("status", internalStatus);

            message.SetValue("channel", channel);
            return message;
        }

        /// <summary>
        /// Loads recorded set of bayeux messages from an input stream.
        /// </summary>
        public int LoadRecording(Stream input)
        {
            return LoadRecording(input, Encoding.UTF8);
        }

        /// <summary>
        /// Loads recorded set of bayeux messages from an input stream.
        /// </summary>
        public int LoadRecording(Stream input, Encoding encoding)
        {
            using (var streamReader = new StreamReader(input, encoding))
            {
                return LoadRecording(streamReader);
            }
        }

        /// <summary>
        /// Loads recorded set of bayeux messages from an input stream-reader.
        /// </summary>
        public int LoadRecording(TextReader input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int addedResponses = 0;

            var reader = new JSonReader(true);

            // if not yet end-of-the-input:
            while (input.Peek() != -1)
            {
                try
                {
                    var jObject = reader.ReadAsJSonMutableObject(input);

                    if (jObject != null)
                    {
                        RecordBayeux("F-" + addedResponses, jObject);
                        addedResponses++;
                    }
                }
                catch (Exception ex)
                {
                    DebugLog.WriteCoreLine("Invalid format of recorded bayeux responses");
                    DebugLog.WriteBayeuxException(ex);
                    break;
                }
            }

            return addedResponses;
        }

        /// <summary>
        /// Load embedded responses from embedded file in calling assembly.
        /// </summary>
        public int LoadEmbeddedRecording(string resourceName)
        {
            return LoadEmbeddedRecording(resourceName, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Load embedded responses from embedded file from given assembly.
        /// </summary>
        public int LoadEmbeddedRecording(string resourceName, Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException("resourceName");

            using (var resource = assembly.GetManifestResourceStream(resourceName))
            {
                return LoadRecording(resource);
            }
        }

        /// <summary>
        /// Loads responses from given content.
        /// </summary>
        public int LoadContentRecording(string content)
        {
            if (string.IsNullOrEmpty(content))
                throw new ArgumentNullException("content");

            using (var streamReader = new StringReader(content))
            {
                return LoadRecording(streamReader);
            }
        }
    }
}
