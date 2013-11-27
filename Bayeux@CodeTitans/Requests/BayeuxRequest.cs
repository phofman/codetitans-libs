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

using System.Globalization;
using CodeTitans.Bayeux.Responses;
using CodeTitans.JSon;

namespace CodeTitans.Bayeux.Requests
{
    /// <summary>
    /// Class wrapping Bayeux message sent to the server.
    /// </summary>
    public class BayeuxRequest : IJSonWritable
    {
        private static long _id;
        private readonly IJSonWritable _data;
        private readonly IJSonWritable _ext;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BayeuxRequest(string channel, IJSonWritable data, IJSonWritable ext)
        {
            FormattedOutput = true;
            RequestMethod = HttpDataSource.MethodPost;
            Channel = channel;

            _id++;
            ID = _id.ToString(CultureInfo.InvariantCulture);
            _data = data;
            _ext = ext;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the ID of this request.
        /// </summary>
        public string ID
        { get; set; }

        /// <summary>
        /// Gets or sets the indication of the client connected to the server.
        /// </summary>
        public string ClientID
        { get; set; }

        /// <summary>
        /// Bayeux channel, where the data should be sent.
        /// </summary>
        public string Channel
        { get; set; }

        /// <summary>
        /// Gets or sets the format style of produced output.
        /// </summary>
        public bool FormattedOutput
        { get; set; }

        /// <summary>
        /// Gets or sets HTTP request method (POST/GET).
        /// </summary>
        public string RequestMethod
        { get; set; }

        /// <summary>
        /// Custom value associated with this request.
        /// </summary>
        public object Tag
        { get; set; }

        /// <summary>
        /// Checks if there is data defined.
        /// </summary>
        protected bool HasData
        {
            get { return _data != null; }
        }

        /// <summary>
        /// Checks if there is ext defined.
        /// </summary>
        protected bool HasExt
        {
            get { return _ext != null; }
        }

        #endregion

        /// <summary>
        /// Gets the JSON string representation of this request.
        /// </summary>
        public override string ToString()
        {
            JSonWriter output = new JSonWriter(FormattedOutput);

            Write(output);
            return output.ToString();
        }

        /// <summary>
        /// Writes additional data associated with this request, that is option for most of them.
        /// </summary>
        protected virtual void WriteOptionalFields(IJSonWriter output)
        {
        }

        /// <summary>
        /// Processes response received from remote server.
        /// </summary>
        public virtual void ProcessResponse(BayeuxResponse response)
        {
        }

        /// <summary>
        /// Process failure response from remote server.
        /// </summary>
        public virtual void ProcessFailed(HttpDataSourceEventArgs e)
        {
        }

        #region IJSonWritable Members

        /// <summary>
        /// Writes content of this object to a JSON stream.
        /// </summary>
        public void Write(IJSonWriter output)
        {
            if (output == null)
                return;

            // create basic JSON representation of Bayeux message:
            // {
            //  channel: "xxx",
            //  clientId: "xxx",
            //  id: "xxx",
            //  ... - optional data inserted by child classes
            // }

            output.WriteObjectBegin();
            output.WriteMember("channel", Channel);
            if (ClientID != null)
                output.WriteMember("clientId", ClientID);
            if (ID != null)
                output.WriteMember("id", ID);

            // write basic Bayeux request data field:
            if (HasData)
            {
                output.WriteMember("data");
                _data.Write(output);
            }

            // write additional (optional) request fields:
            WriteOptionalFields(output);

            // write the data extension:
            if (HasExt)
            {
                output.WriteMember("ext");
                _ext.Write(output);
            }
            output.WriteObjectEnd();
        }

        #endregion
    }
}
