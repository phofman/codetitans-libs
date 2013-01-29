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
using CodeTitans.JSon;

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Wrapper class over Bayeux advice.
    /// </summary>
    public sealed class BayeuxAdvice : IJSonWritable
    {
        /// <summary>
        /// Init constructor.
        /// </summary>
        public BayeuxAdvice(IJSonObject data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Contains("reconnect"))
                Reconnect = ParseReconnect(data["reconnect"].StringValue);

            if (data.Contains("interval"))
                Interval = data["interval"].Int32Value;

            Data = data;
        }

        #region Properties

        /// <summary>
        /// Reconnection action requested by server.
        /// </summary>
        public BayeuxAdviceReconnectType Reconnect
        { get; private set; }

        /// <summary>
        /// Gets the reconnect interval.
        /// </summary>
        public int Interval
        { get; private set; }

        /// <summary>
        /// Raw advice data.
        /// </summary>
        public IJSonObject Data
        { get; private set; }

        #endregion

        private static BayeuxAdviceReconnectType ParseReconnect(string reconnectString)
        {
            if (string.Compare(reconnectString, "none", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxAdviceReconnectType.None;

            if (string.Compare(reconnectString, "handshake", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxAdviceReconnectType.Handshake;

            if (string.Compare(reconnectString, "retry", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxAdviceReconnectType.Retry;

            return BayeuxAdviceReconnectType.None;
        }

        private static string ToString(BayeuxAdviceReconnectType reconnect)
        {
            if (reconnect == BayeuxAdviceReconnectType.None)
                return "none";

            if (reconnect == BayeuxAdviceReconnectType.Retry)
                return "retry";

            if (reconnect == BayeuxAdviceReconnectType.Handshake)
                return "handshake";

            return null;
        }

        #region IJSonWritable Members

        /// <summary>
        /// Writes JSON representation of this object.
        /// </summary>
        public void Write(IJSonWriter output)
        {
            output.WriteObjectBegin();
            output.WriteMember("reconnect", ToString(Reconnect));
            output.WriteMember("interval", Interval);
            output.WriteObjectEnd();
        }

        #endregion
    }
}
