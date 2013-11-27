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

namespace CodeTitans.Bayeux
{
    /// <summary>
    /// Types of connection handled by Bayeux server and client.
    /// </summary>
    [Flags]
    public enum BayeuxConnectionTypes
    {
        /// <summary>
        /// Connection supports nothing.
        /// </summary>
        None = 0,
        /// <summary>
        /// Connection supports long waiting for asynchronous response (events).
        /// </summary>
        LongPolling = 1,
        /// <summary>
        /// ?
        /// </summary>
        CallbackPolling = 2,
        /// <summary>
        /// ?
        /// </summary>
        Iframe = 4,
        /// <summary>
        /// ?
        /// </summary>
        Flash = 8,
        /// <summary>
        /// Connection will respond to each request.
        /// </summary>
        RequestResponse = 16
    }

    /// <summary>
    /// Helper class to support conversions to/from string objects.
    /// </summary>
    public static class BayeuxConnectionTypesHelper
    {
        /// <summary>
        /// Converts the <see cref="BayeuxConnectionTypes"/> value into a string.
        /// If it is a combination of few values, then 'null' will be returned.
        /// </summary>
        public static string ToString(BayeuxConnectionTypes type)
        {
            if (type == BayeuxConnectionTypes.LongPolling)
                return "long-polling";
            if (type == BayeuxConnectionTypes.CallbackPolling)
                return "callback-polling";
            if (type == BayeuxConnectionTypes.Iframe)
                return "iframe";
            if (type == BayeuxConnectionTypes.Flash)
                return "flash";
            if (type == BayeuxConnectionTypes.RequestResponse)
                return "request-response";

            return null;
        }

        /// <summary>
        /// Converts <see cref="BayeuxConnectionTypes"/> flag fields into a collection of strings.
        /// </summary>
        public static string[] ToCollection(BayeuxConnectionTypes types)
        {
            var result = new List<string>();

            if ((types & BayeuxConnectionTypes.LongPolling) == BayeuxConnectionTypes.LongPolling)
                result.Add("long-polling");
            if ((types & BayeuxConnectionTypes.CallbackPolling) == BayeuxConnectionTypes.CallbackPolling)
                result.Add("callback-polling");
            if ((types & BayeuxConnectionTypes.Iframe) == BayeuxConnectionTypes.Iframe)
                result.Add("iframe");
            if ((types & BayeuxConnectionTypes.Flash) == BayeuxConnectionTypes.Flash)
                result.Add("flash");
            if ((types & BayeuxConnectionTypes.RequestResponse) == BayeuxConnectionTypes.RequestResponse)
                result.Add("request-response");

            return result.ToArray();
        }

        /// <summary>
        /// Converts a string into a <see cref="BayeuxConnectionTypes"/> value.
        /// </summary>
        public static BayeuxConnectionTypes Parse(string type)
        {
            if (string.Compare(type, "long-polling", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxConnectionTypes.LongPolling;

            if (string.Compare(type, "callback-polling", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxConnectionTypes.CallbackPolling;

            if (string.Compare(type, "iframe", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxConnectionTypes.Iframe;

            if (string.Compare(type, "flash", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxConnectionTypes.Flash;

            if (string.Compare(type, "request-response", StringComparison.OrdinalIgnoreCase) == 0)
                return BayeuxConnectionTypes.RequestResponse;

            //throw new ArgumentOutOfRangeException("type");
            return BayeuxConnectionTypes.None;
        }
    }
}
