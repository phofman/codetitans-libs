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
using System.Globalization;
using CodeTitans.JSon;

namespace CodeTitans.Bayeux.Responses
{
    /// <summary>
    /// Response returned by Bayeux server.
    /// </summary>
    public class BayeuxResponse : IJSonSerializable
    {
        private IJSonObject _jsonResponse;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxResponse(IJSonObject data)
        {
            Read(data);
        }

        #region Properties

        /// <summary>
        /// ID of the message.
        /// This value is optional for asynchronous events.
        /// </summary>
        public string ID
        { get; protected set; }

        /// <summary>
        /// Receiver ClientID.
        /// </summary>
        public string ClientID
        { get; protected set; }

        /// <summary>
        /// Channel over which response is valid.
        /// </summary>
        public string Channel
        { get; protected set; }

        /// <summary>
        /// Result of the operation.
        /// </summary>
        public bool Successful
        { get; protected set; }

        /// <summary>
        /// Optional operation timestamp.
        /// </summary>
        public DateTime Timestamp
        { get; protected set; }

        /// <summary>
        /// Error description.
        /// </summary>
        public BayeuxError Error
        { get; protected set; }

        /// <summary>
        /// Additional server connection advice in case of errors or resource problems on server side.
        /// </summary>
        public BayeuxAdvice Advice
        { get; protected set; }

        /// <summary>
        /// Response data.
        /// </summary>
        public IJSonObject Data
        { get; protected set; }

        /// <summary>
        /// Extension data passed optionally.
        /// </summary>
        public IJSonObject Ext
        { get; protected set; }

        #endregion

        #region IJSonReadable Members

        /// <summary>
        /// Reads additional fields associated with this response while parsing.
        /// </summary>
        protected virtual void ReadOptionalFields(IJSonObject input)
        {
        }

        /// <summary>
        /// Reads the response from given JSON object.
        /// </summary>
        public void Read(IJSonObject input)
        {
            _jsonResponse = input;

            // reset all fields to their default values:
            Channel = null;
            ClientID = null;
            Successful = false;
            Timestamp = DateTime.MinValue;
            ID = null;
            Data = null;
            Error = null;
            Advice = null;
            Ext = null;

            // stop processing, if there is no input data given:
            if (input == null)
                return;

            // now read the Bayeux mandatory fields:
            Channel = input["channel"].StringValue;
            if (!BayeuxChannel.IsValid(Channel))
                throw new FormatException("Invalid channel format");


            if (input.Contains("clientId"))
                ClientID = input["clientId"].StringValue;

            // for meta channels, this field is required,
            // however it's optional for others (like events)
            if (Channel.StartsWith("/meta"))
                Successful = input["successful"].BooleanValue;
            else
            {
                if (input.Contains("successful"))
                    Successful = input["successful"].BooleanValue;
            }

            // parse optional fields:
            if (input.Contains("timestamp"))
                Timestamp = input["timestamp"].DateTimeValue;
            if (input.Contains("id"))
                ID = input["id"].StringValue;
            if (input.Contains("data"))
                Data = input["data"];
            if (input.Contains("error"))
                Error = new BayeuxError(input["error"].StringValue);
            if (input.Contains("advice"))
                Advice = new BayeuxAdvice(input["advice"]);
            if (input.Contains("ext"))
                Ext = input["ext"];

            ReadOptionalFields(input);
        }

        #endregion

        #region IJSonWritable Members

        /// <summary>
        /// Writes additional fields related to this response.
        /// </summary>
        protected virtual void WriteOptionalFields(IJSonWriter output)
        {
        }

        /// <summary>
        /// Writes content of this object to a JSON stream.
        /// </summary>
        public void Write(IJSonWriter output)
        {
            if (output == null)
                return;

            output.WriteObjectBegin();
            output.WriteMember("channel", Channel);
            if (ID != null)
                output.WriteMember("id", ID);
            output.WriteMember("clientId", ClientID);
            output.WriteMember("successful", Successful ? 1 : 0);
            if (Data != null)
            {
                output.WriteMember("data");
                output.Write(Data);
            }
            if (Ext != null)
            {
                output.WriteMember("ext");
                output.Write(Ext);
            }

            // writes additional members defined by child classes:
            WriteOptionalFields(output);
            output.WriteObjectEnd();
        }

        #endregion

        /// <summary>
        /// Gets string representation of this object.
        /// </summary>
        public override string ToString()
        {
            IJSonWriter output = new JSonWriter(true);

            output.Write(_jsonResponse);
            return output.ToString();
        }
    }
}
